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

                string python_code = ExportSensorToPython(sensor);
                string python_path = Path.Combine(tb_path.Text, $"{sensor.Name}.py");
                ExportCode(python_path, python_code);

                string xml_code = ExportSensorToXml(sensor);
                string xml_path = Path.Combine(tb_path.Text, $"{sensor.Name}.xml");
                ExportCode(xml_path, xml_code);
            }

            Logger.LogInfoToTextBox(this, "Script finished.");
        }

        private void ExportCode(string path, string code)
        {
            Logger.LogInfoToTextBox(this, $"Save code to <{path}>.");
            System.IO.File.WriteAllText(path, code);
        }

        private string ExportSensorToXml(Element sensor)
        {
            string name = sensor.Name;

            return $@"<?xml version=""1.0""?>
<dds xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
     xsi:noNamespaceSchemaLocation=""https://community.rti.com/schema/current/rti_dds_qos_profiles.xsd"">

    <qos_library name=""QosLibrary"">
        <qos_profile name=""DefaultProfile""
                     base_name=""BuiltinQosLib::Generic.StrictReliable""
                     is_default_qos=""true"">
            <domain_participant_qos>
                <participant_name>
                    <name>{name}</name>
                </participant_name>
            </domain_participant_qos>
        </qos_profile>
    </qos_library>

    <types>
        <struct name=""Measurement"" extensibility=""extensible"">
            <member name=""value"" type=""float""/>
            <member name=""time"" type=""long""/>
            <member name=""host"" stringMaxLength=""128"" type=""string""/>
            <member name=""sensor_name"" stringMaxLength=""128"" type=""string""/>
        </struct>
    </types>

    <domain_library name=""SensorDomainLibrary"">
        <domain name=""SensorDomain"" domain_id=""0"">
            <register_type name=""Measurement"" type_ref=""Measurement"" />
	    <topic name=""SensorData"" register_type_ref=""Measurement""/>
        </domain>
    </domain_library>

    <domain_participant_library name=""SensorParticipantLibrary"">
        <domain_participant name=""SensorParticipant"" domain_ref=""SensorDomainLibrary::SensorDomain"">
        <publisher name=""sensor_data_publisher"">
		    <data_writer name=""sensor_data_writer"" topic_ref=""SensorData""/>
		</publisher>

        </domain_participant>
    </domain_participant_library>
</dds>
";
        }

        private string ExportSensorToPython(Element sensor)
        {
            string name = sensor.Name;
            string xml_path = $"./{name}.xml";
            string minValue = sensor.TaggedValues.GetByName("MinValue").Value;
            string maxValue = sensor.TaggedValues.GetByName("MaxValue").Value;
            string sleep = sensor.TaggedValues.GetByName("Sleep").Value;

            return $@"from datetime import datetime, timedelta
from time import sleep, time
import random
import rticonnextdds_connector as rti

connector = rti.Connector(url='{xml_path}', config_name='SensorParticipantLibrary::SensorParticipant')

while True:
    output = connector.get_output('sensor_data_publisher::sensor_data_writer')
    output.instance['time'] = time()
    output.instance['host'] = 'simulator'
    output.instance['sensor_name'] = '{name}'
    output.instance['value'] = random.uniform({minValue}, {maxValue})
    output.write()
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
