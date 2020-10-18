namespace scanner.Hubs
{
    public class ScannerOptions
    {
        public string SharePath { get; set; }
        public string ScannerMode { get; set; } = "Lineart";
        public string FileFormat { get; set; } = "png";
        public bool Duplex { get; set; }
    }
}