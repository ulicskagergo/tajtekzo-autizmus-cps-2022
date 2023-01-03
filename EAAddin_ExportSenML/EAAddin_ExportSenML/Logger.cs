namespace EAAddin_ExportSenML
{
    class Logger
    {
        public static void LogDebugToTextBox(ExportSenMLUserControl form, string msg)
        {
            form.WriteToTextBox($" [ DEBUG ] {msg}\r\n");
        }

        public static void LogInfoToTextBox(ExportSenMLUserControl form, string msg)
        {
            form.WriteToTextBox($" [ INFO ] {msg}\r\n");
        }

        public static void LogErrorToTextBox(ExportSenMLUserControl form, string msg)
        {
            form.WriteToTextBox($" [ WARNING ] {msg}\r\n");
        }
    }
}
