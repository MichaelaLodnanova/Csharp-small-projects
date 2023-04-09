using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW02.BussinessContext.DB.EShopAdminUI;
using HW02.BussinessContext.DB.Entities;
using Microsoft.Extensions.Primitives;

namespace HW02.BussinessContext.EShopAdminUI
{
    public class CommandParsedEventArgs : EventArgs
    {
        public CommandType logType { get; set; }
        public EntityType entityType { get; set; }
        public bool success { get; set; }
        public string? errorMessage { get; set; }
        public Entity? entity { get; set; }

        public string GetLogContent()
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;
            string timestamp = "[" + now.ToString("dd/MM/yyyy HH:mm:ss") + "]";
            sb.Append(timestamp);
            sb.Append($" {logType}");
            sb.Append($"; {entityType}");
            sb.Append($"; {(success ? "Success" : "Failure")}");
            if (!success)
            {
                sb.Append($"; {errorMessage}");
            }
            else
            {
                switch (entity)
                {
                    case Category:
                        sb.Append($"; {entity.Id}");
                        sb.Append($"; {entity.Name}");
                        break;
                    case Product:
                        sb.Append($"; {entity.Id}");
                        sb.Append($"; {entity.Name}");
                        sb.Append($"; {entity.CategoryId}");
                        break;
                }
            }
            return sb.ToString();
        }
    }
}

