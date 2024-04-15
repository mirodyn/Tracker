using EConnect;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tracker.QiTools
{
    internal class QiDataAccess
    {

        private readonly QIConnectorConfig settings;
        private TEConnect qi;

        public QiDataAccess(QIConnectorConfig settings)
        {
            this.settings = settings;
            this.qi = new EConnect.TEConnect();
        }

        public bool LogIn()
        {
            int iRet;
            try
            {
                iRet = (int)qi.Connect(settings.Server,
                                       settings.Port,
                                       settings.User,
                                       settings.Password);

                return true;

            }
            catch (Exception ex)
            {

                MessageBox.Show("Exception while connecting to QI! Exception: " + ex.Message);
                throw;
            }

            if (iRet == -1)
            {
                MessageBox.Show("Error while connecting to QI! QI returned: -1 ");
                return false;
            }
        }

        public void LogOut()
        {
            if (qi != null)
            {
                qi.Disconnect();
                qi = null;
            }
        }

        public bool WriteTaskTimeRangeToQi(TaskTimeRange timeRange,Config configuration)
        {
            TEDataSet qids = (TEDataSet)qi.CreateDS(settings.NewActivityDatasetIcu);
            try
            {

                qids.SetParam("FILTER", settings.NewActivityDatasetParam);
                qids.SetParam("PARAMS", "14462754,10='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                qids.OpenEmpty();
                qids.Insert();
                qids.SetFieldANSIValue(settings.NewActivityActivityNameIcu, timeRange.LinkedTask.Name);

                qids.SetFieldANSIValue(settings.NewActivityAnnotationIcu, timeRange.Annotation + "\n<tracker:" + timeRange.TimeRangeGuid + ">");

                //od
                qids.SetFieldANSIValue(settings.NewActivityActivityFromIcu, timeRange.From.ToString("yyyy-MM-dd HH:mm:ss"));
               
                //do
                qids.SetFieldANSIValue(settings.NewActivityActivityToIcu, timeRange.To.ToString("yyyy-MM-dd HH:mm:ss"));
                
                //Zadavatel
                qids.SetFieldANSIValue("12403686,10", "Jiří Smutný");
                qids.SetFieldANSIValue("12401382,10.ic_b", "7843975");
                qids.SetFieldANSIValue("12401382,10.u_b", "11070");
                qids.SetFieldANSIValue("12403683,10.ic", "7843975");
                qids.SetFieldANSIValue("12403683,10.u", "11070");
                
                //Relalizátor
                qids.SetFieldANSIValue("7478552,10", "Jiří Smutný");
                qids.SetFieldANSIValue("7478543,10.ic", "7843975");
                qids.SetFieldANSIValue("7478543,10.u", "11070");
                qids.SetFieldANSIValue("7478541,10.ic_b", "7843975");
                qids.SetFieldANSIValue("7478541,10.u_b", "11070");

                //Nadrizena akce
                qids.SetFieldANSIValue("7504031,10.ic", ((Task)timeRange.LinkedTask).Icu.Split(',')[0]);
                qids.SetFieldANSIValue("7504031,10.u", ((Task)timeRange.LinkedTask).Icu.Split(',')[1]);
                qids.SetFieldANSIValue("7504026,10.ic_a", ((Task)timeRange.LinkedTask).Icu.Split(',')[0]);
                qids.SetFieldANSIValue("7504026,10.u_a", ((Task)timeRange.LinkedTask).Icu.Split(',')[1]);

                //Kod akce
                qids.SetFieldANSIValue("7504072,10", timeRange.LinkedTask.QiCode);

                //Nazev akce
                qids.SetFieldANSIValue("7504073,10", timeRange.LinkedTask.Name);

                qids.Post();
                qids.Close();
                qids.DestroyDS();
            }
            catch (Exception e)
            {
                File.AppendAllText("tracker.log", "[" + DateTime.Now.ToString() + "]ERR: " + e.Message + "\n");
                qids?.Close();
                qids?.DestroyDS();
                return false;
            }

            return true;
        }

        public bool WriteNewTaskTimeRangeToQi(TaskTimeRange timeRange,Config configuration)
        {
            TEDataSet qids = (TEDataSet)qi.CreateDS(settings.NewActivityDatasetIcu);
            try
            {
                
                qids.SetParam("FILTER", settings.NewActivityDatasetParam);
                qids.SetParam("PARAMS", "14462754,10='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                qids.OpenEmpty();
                qids.Insert();
                qids.SetFieldANSIValue(settings.NewActivityActivityNameIcu, timeRange.LinkedTask.Name);
                //popis
                qids.SetFieldANSIValue(settings.NewActivityAnnotationIcu, timeRange.Annotation + "\n<tracker:" +timeRange.TimeRangeGuid +">");
                
                //od
                qids.SetFieldANSIValue(settings.NewActivityActivityFromIcu,timeRange.From.ToString("yyyy-MM-dd HH:mm:ss"));
                
                //do
                qids.SetFieldANSIValue(settings.NewActivityActivityToIcu, timeRange.To.ToString("yyyy-MM-dd HH:mm:ss"));

                //Zadavatel
                qids.SetFieldANSIValue("12403686,10", "Jiří Smutný");
                qids.SetFieldANSIValue("12401382,10.ic_b", "7843975");
                qids.SetFieldANSIValue("12401382,10.u_b", "11070");
                qids.SetFieldANSIValue("12403683,10.ic", "7843975");
                qids.SetFieldANSIValue("12403683,10.u", "11070");

                //Relalizátor
                qids.SetFieldANSIValue("7478552,10", "Jiří Smutný");
                qids.SetFieldANSIValue("7478543,10.ic", "7843975");
                qids.SetFieldANSIValue("7478543,10.u", "11070");
                qids.SetFieldANSIValue("7478541,10.ic_b", "7843975");
                qids.SetFieldANSIValue("7478541,10.u_b", "11070");

                qids.Post();
                qids.Close();
                qids.DestroyDS();
            }
            catch (Exception e)
            {
                File.AppendAllText("tracker.log", "[" + DateTime.Now.ToString() + "]ERR: " + e.Message + "\n");
                qids?.Close();
                qids?.DestroyDS();
                return false;
            }

            return true;
        }

        public List<string[]> ReadTasksFromDataset()
        {
            List<string[]> list = new List<string[]>();
            TEDataSet ds = (TEDataSet)qi.CreateDS(settings.ActiveTasksDatasetIcu);
            if (ds == null) return list;
            ds.SetParam("PARAMS", settings.ActiveTasksDatasetParam);
            ds.SetParam("FILTER", settings.GetPopulatedActiveTasksFilterString());
            ds.Open();
            int cnt = (int)ds.RecordCount();

            for (int i = 1; i<= cnt; i++)
            {
                string[] row = new string[4]; 
                row[0] = (string)ds.GetFieldValue(settings.TaskNameIcu);
                row[1] = (string)ds.GetFieldValue(settings.QiCodeIcu);
                row[2] = (string)ds.GetFieldValue(settings.CustomerNameIcu);
                row[3] = (string)ds.GetFieldValue("ic") + "," + (string)ds.GetFieldValue("u");
                list.Add(row);
                ds.Next();
            }
            ds.Close();

            return list;
        }

       

    }
}
