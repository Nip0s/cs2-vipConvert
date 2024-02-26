using System.Data;
using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Config;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Iks_ASConvert;
using MySqlConnector;


namespace Iks_VIPConvert;

public class vipConvert : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "vipConvert";

    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks x nipos";

    private string _dbConnectionString = "";

   public PluginConfig Config { get; set; }

    public void OnConfigParsed(PluginConfig config)
    {
        _dbConnectionString = "Server=" + config.host + ";Database=" + config.database
                              + ";port=" + config.port + ";User Id=" + config.user + ";password=" + config.pass;

        Task.Run(async () =>
        {
            await SetFlagsToVips();
        });
        Config = config;
    }

    public async Task SetFlagsToVips()
    {
        List<Admin> admins = new List<Admin>();
        string sql = $@"SELECT * FROM vip_users WHERE expires>{DateTimeOffset.UtcNow.ToUnixTimeSeconds()} OR expires=0;";
        try
        {
            using (var connection = new MySqlConnection(_dbConnectionString))
            {
                connection.Open();
                var comm = new MySqlCommand(sql, connection);
                var reader = await comm.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    admins.Add(new Admin(reader.GetInt32("account_id").ToString(), reader.GetString("group"), reader.GetInt32("sid")));
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($" [vipConvert] Db error: {ex}");
        }

        Server.NextFrame(() =>
        {
            SetVipFlags(admins);
        });
    }


    public void SetVipFlags(List<Admin> admins)
    {
        foreach (var admin in admins)
        {
            if (admin.Sid != Config.sid) continue;
            foreach (var vipflags in Config.ConvertVips)
            {
                if (admin.VipFlags.Contains(vipflags.Key))
                {
                    AdminManager.AddPlayerPermissions(admin.Steamid, vipflags.Value.ToArray());
                    Console.WriteLine($"[vipConvert] Admin {admin.Steamid.ToString()} converted to {string.Join(", ", vipflags.Value)}");
                }
            }
        }
    }

    [CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY)]
    [ConsoleCommand("css_vip_convert")]
    public void OnConvertCommand(CCSPlayerController? controller, CommandInfo info)
    {
        Task.Run(async () =>
        {
            await SetFlagsToVips();
        });
    }
}
