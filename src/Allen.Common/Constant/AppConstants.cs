namespace Allen.Common;

public static class AppConstants
{
	public static readonly string DatabaseName = "Allen";
	public static readonly string DefaultUri = "https://localhost:7070";
	public static readonly string DefaultGroupPicture = "";
	public static readonly string DefaultUserPicture = "";
	public const int DefaultPageTop = 10;
	public const int DefaultPageSkip = 0;
	public const bool DefaultNeedTotalCount = false;
	public const int MaxPageSize = 100;
	public const int DefaultPage = 1;
	public const string DefaultOrderBy = "CreatedAt";
	public const string DefaultOrderType = "DESC";
	public const string SqlServerConnection = "SqlServerConnection";
	public const string MongoDbConnection = "MongoDbConnection";
	public const string RedisConnection = "RedisConnection";
	public const string AzureBlobStorage = "AzureBlobStorage";
	public const int DefaultMinLength = 1;

	// Default max length 
	public const int MaxLengthTopic = 100;
	public const int MaxLengthPermision = 100;
	public const int MaxLengthName = 30;
	public const int MaxLengthEmail = 320;
	public const int MaxLengthDescription = 1000;
	public const int MaxLengthHastTag = 50;
	public const int MaxLengthUrl = 2000;
	public const int MaxLengthSearchQuery = 150;

	// Default media file size
	public const long MaxBlobSize = 1024 * 1024 * 50;
	public const string BlobContainer = "allen";
    public const string BlobContainerWriting = "ieltswritings";
    public const string BlobContainerFileMp3 = "allenmp3";
	public const string BlobContainerListeningFile = "allen-listening";
	public const string BlobContainerFlashCard = "flashcards-media";

    public static readonly string[] AllowedImageMimeTypes = new[]
   {
        "image/jpeg",
        "image/jpg",
        "image/png"
    };

    // Các định dạng tệp âm thanh được phép
    public static readonly string[] AllowedAudioMimeTypes = new[]
    {
        "audio/mpeg" // .mp3
    };

    // Các định dạng tệp video được phép
    public static readonly string[] AllowedVideoMimeTypes = new[]
    {
        "video/mp4",
        "video/x-matroska" // .mkv
    };

    public const string FileTooLargeMessage = "File size must not exceed 5 MB.";
    public const string InvalidImageFormatMessage = "Only JPEG, PNG formats are allowed.";
    public const string InvalidAudioFormatMessage = "Only MP3 formats are allowed.";
    public const string InvalidVideoFormatMessage = "Only MP4 and MKV video formats are allowed.";
    //public const string BlobContainer = "posbh";
}
