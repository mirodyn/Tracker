using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tracker.QiTools
{
    public class QIConnectorConfig
    {
        public string FullName { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        //Active tasks dataset properties
        public string ActiveTasksDatasetIcu { get; set; }
        public string ActiveTasksDatasetParam { get; set; }
        public string ActiveTasksDatasetFilter { get; set; }

        public string QiCodeIcu { get; set; }
        public string TaskNameIcu { get; set; }
        public string CustomerNameIcu { get; set; }

        //Create tasks dataset properties
        public string NewActivityDatasetIcu { get; set; }
        public string NewActivityFullNameIcu { get; set; }
        public string NewActivityCurrentTimeIcu { get; set; }
        public string NewActivityAvailableIcu { get; set; }
        public string NewActivityQiCodeIcu { get; set; }
        public string NewActivityTaskNameIcu { get; set; }
        public string NewActivityActivityNameIcu { get; set; }
        public string NewActivityActivityFromIcu { get; set; }
        public string NewActivityTaskFromIcu { get; set; }
        public string NewActivityActivityToIcu { get; set; }
        public string NewActivityTaskToIcu { get; set; }
        public string NewActivityAnnotationIcu { get; set; }
        public string NewActivityAttachedTaskIcu { get; set; }
        public string NewActivityPlannedByIcu { get; set; }
        public string NewActivityDatasetParam { get; set; }

        //Create activities dataset properties
        //todo
        public QIConnectorConfig()
        {
            FullName = "Novák Jan";
            Server = "qi.brno.solidvision.cz";
            Port = "33445";

            ActiveTasksDatasetIcu = "35955,10";
            ActiveTasksDatasetParam = "37810816,10=862980\n37810818,10=9";
            ActiveTasksDatasetFilter = "\"482752,10\" like '{0}' AND (\"35970,10\" = 0 OR \"35970,10\" = 6 OR \"35970,10\" = 1)";

            TaskNameIcu = "35966,10";
            QiCodeIcu = "35967,10";
            CustomerNameIcu = "66510,11110";



            NewActivityDatasetParam = "\"7478552,10\" = 'Smutný Jiří'";
            NewActivityDatasetIcu = "7478409,10";
            NewActivityActivityNameIcu = "7478413,10";
            NewActivityActivityFromIcu = "7478412,10";
            NewActivityActivityToIcu = "7478411,10";




            NewActivityFullNameIcu = "10162637,10";
            NewActivityCurrentTimeIcu = "12401783,10";
            NewActivityAvailableIcu = "26558499,10";
            NewActivityQiCodeIcu = "8080808,10";
            NewActivityTaskNameIcu = "8080811,10";
            NewActivityTaskFromIcu = "8080809,10";
            NewActivityTaskToIcu = "8080810,10";
            NewActivityAttachedTaskIcu = "8138770,10";
            NewActivityAnnotationIcu = "7677155,10";
        }


        public string GetUnecryptedPassword()
        {
            return "";
        }

        public string GetPopulatedActiveTasksFilterString()
        {
            return string.Format(ActiveTasksDatasetFilter, FullName);
        }

        public bool CheckConfig()
        {
            return (!string.IsNullOrEmpty(FullName))
                && (!string.IsNullOrEmpty(Server))
                && (!string.IsNullOrEmpty(Port))
                && (!string.IsNullOrEmpty(User))
                && (!string.IsNullOrEmpty(Password))
                && (!string.IsNullOrEmpty(ActiveTasksDatasetIcu))
                && (!string.IsNullOrEmpty(ActiveTasksDatasetParam))
                && (!string.IsNullOrEmpty(ActiveTasksDatasetFilter))
                && (!string.IsNullOrEmpty(QiCodeIcu))
                && (!string.IsNullOrEmpty(TaskNameIcu))
                && (!string.IsNullOrEmpty(CustomerNameIcu));
        }


    }
}