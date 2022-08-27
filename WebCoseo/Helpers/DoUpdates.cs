using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WebCoseo.AppServices.Updates;

namespace WebCoseo.Helpers
{
    public class DoUpdates
    {
        public static DoUpdates _instance;
        public DoUpdates()
        {
            _instance = this;
        }

        public async Task Run()
        {
            // Load up each updates

            // 0.1.0 Beta
            await Task.Run(async () =>
            {
                using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
                {
                    var getversion = context.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version").Single();
                    var getversioncheck = context.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version_check").Single();
                    int versioncvalue = int.Parse(getversioncheck.ConfigValue);


                    if (versioncvalue >= 000100 && versioncvalue < 000101)
                    {
                        /*
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SplashScreenWindow._instance.Running_Updates("Running Update 0.1.0");
                        });
                        */
                        var update_00_01_00 = new Update_00_01_00();
                        await update_00_01_00.RunUpdate();
                    }


                }

            });

        }
    }
}
