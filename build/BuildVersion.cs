namespace BuildScript
{
    public class BuildVersion
    {
        public BuildVersion(int major, int minor, int patch, int quality, string suffix)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Quality = quality;
        }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public int Quality { get; set; }

        public string Suffix { get; set; }

        public string Version()
        {
            return $"{Major}.{Minor}.{Patch}.{Quality}";
        }

        public string VersionWithSuffix()
        {
            return Version() + (Suffix == null ? string.Empty : $"-{Suffix}");
        }
    }
}
