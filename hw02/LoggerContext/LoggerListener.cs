using HW02.BussinessContext.EShopAdminUI;
using HW02.LoggerContext.DB;

namespace HW02
{
    public class LoggerListener
    {
        public readonly LoggerDBContext loggerDB;

        public LoggerListener(LoggerDBContext loggerDB)
        {
            this.loggerDB = loggerDB;
        }
        public void OnCommandProcessed(Object sender, CommandParsedEventArgs e)
        {
            loggerDB.WriteLog(e);
        }
    }
}
