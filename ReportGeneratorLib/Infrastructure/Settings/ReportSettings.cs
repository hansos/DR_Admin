using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGeneratorLib.Infrastructure.Settings
{
    public class ReportSettings
    {
        public string Provider { get; set; } = string.Empty;
        public FastReport? FastReport { get; set; }
        public QuestPdf? QuestPdf { get; set; }
    }
}
