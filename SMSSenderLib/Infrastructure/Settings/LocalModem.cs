namespace SMSSenderLib.Infrastructure.Settings
{
    public class LocalModem
    {
        public string ComPort { get; set; } = "COM1";
        public int BaudRate { get; set; } = 115200;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = "None"; // None, Odd, Even, Mark, Space
        public string StopBits { get; set; } = "One"; // None, One, Two, OnePointFive
        public int ReadTimeout { get; set; } = 3000;
        public int WriteTimeout { get; set; } = 3000;
        public string? Pin { get; set; }
    }
}
