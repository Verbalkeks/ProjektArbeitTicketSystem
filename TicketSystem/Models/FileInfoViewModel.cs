namespace TicketSystem.Models
{
    public class FileInfoViewModel
    {
        public int Id { get; set; }
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime UploadedAt { get; set; }
        public int TicketId { get; set; }
        public string FileSizeNormalized => NormalizeFileSize(FileSize);

        private string NormalizeFileSize(long fileSizeInBytes)
        {
            string[] units = ["Byte", "KiB", "MiB", "GiB", "TiB"];
            var unit = 0;
            double fileSize = fileSizeInBytes;
            while (fileSize > 1024)
            {
                fileSize /= 1024;
                unit++;
            }
            return $"{fileSize:#.00} {units[unit]}";
        }
    }
}
