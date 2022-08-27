using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Updates
{
    public class Update_00_01_00
    {
        public static Update_00_01_00 _instance;
        public Update_00_01_00()
        {
            _instance = this;
        }

        public async Task RunUpdate()
        {

            await Task.Run(() =>
            {
                // in this update
                //=========================================================================
                using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
                {
                    var getversion = context.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version").Single();
                    var getversioncheck = context.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version_check").Single();
                    var getVersionStability = context.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version_stability").Single();
                    
                    int versioncvalue = int.Parse(getversioncheck.ConfigValue);
                    if (versioncvalue >= 000100 && versioncvalue < 000101)
                    {
                        getversion.ConfigValue = "0.1.0";
                        getversioncheck.ConfigValue = "000100";
                        getVersionStability.ConfigValue = "Beta";
                        context.SaveChanges();
                    }


                }
            });

        }
    }
}
