using EA;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EAAddin_ExportSenML
{
    public partial class ExportSenMLUserControl : UserControl
    {
        public Repository Repository { get; set; }

        private void btn_export_Click(object sender, EventArgs e)
        {
            tb.Text = "";
            Logger.LogInfoToTextBox(this, "Script started.");

            if (Repository.GetTreeSelectedItemType() != ObjectType.otElement)
            {
                Logger.LogErrorToTextBox(this, "Cannot be initialized. Wrong object selected.");
                return;
            }

            Element treeSelectedElement = Repository.GetTreeSelectedObject();
            if (treeSelectedElement.Type != "Class")
            {
                Logger.LogErrorToTextBox(this, "Cannot be initialized. Wrong object selected.");
                return;
            }

            foreach (EA.Connector connector in treeSelectedElement.Connectors)
            {
                Element sensor = Repository.GetElementByID(connector.ClientID);
                string code = ExportSensorToPython(sensor);
                string path = Path.Combine(tb_path.Text, $"{sensor.Name}.py");
                Logger.LogInfoToTextBox(this, $"Save code to <{path}>.");
                Logger.LogInfoToTextBox(this, code);
                System.IO.File.WriteAllText(path, code);
            }

            Logger.LogInfoToTextBox(this, "Script finished.");
        }

        private string ExportSensorToPython(Element sensor)
        {
            string name = sensor.Name;
            string url = sensor.TaggedValues.GetByName("URL").Value;
            string token = sensor.TaggedValues.GetByName("Token").Value;
            string org = sensor.TaggedValues.GetByName("Org").Value;
            string bucket = sensor.TaggedValues.GetByName("Bucket").Value;
            string writeOptions = sensor.TaggedValues.GetByName("WriteOptions").Value;
            string minValue = sensor.TaggedValues.GetByName("MinValue").Value;
            string maxValue = sensor.TaggedValues.GetByName("MaxValue").Value;
            string sleep = sensor.TaggedValues.GetByName("Sleep").Value;

            return $@"from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS, ASYNCHRONOUS
from datetime import datetime, timedelta
from time import sleep
import pandas as pd
import random

client = InfluxDBClient(url=""{url}"", token=""{token}"", org=""{org}"")
writer = client.write_api(write_options={writeOptions})
while True:
    point = Point(""modbus"")
    point.time(datetime.now())
    point.tag(""host"", ""simulator"")
    point.tag(""sensor_name"", ""{name}"")
    point.field(""value"", random.uniform({minValue}, {maxValue}))
    writer.write(bucket=""{bucket}"", record=point)
    sleep({sleep})";
        }

        public void WriteToTextBox(string msg)
        {
            tb.AppendText(msg);
            tb.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExportSenMLHelp stateMachineExportHelp = new ExportSenMLHelp();
            stateMachineExportHelp.Show();
        }

        public ExportSenMLUserControl()
        {
            InitializeComponent();
        }
    }
}
