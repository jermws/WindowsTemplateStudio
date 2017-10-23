namespace CognitiveServicesUWP.Models
{
    public class HandWritingImageResponse
    {
        public string status { get; set; }
        public bool succeeded { get; set; }
        public bool failed { get; set; }
        public bool finished { get; set; }
        public Recognitionresult recognitionResult { get; set; }
    }

    public class Recognitionresult
    {
        public HWLine[] lines { get; set; }
    }

    public class HWLine
    {
        public int[] boundingBox { get; set; }
        public string text { get; set; }
        public HWWord[] words { get; set; }
    }

    public class HWWord
    {
        public int[] boundingBox { get; set; }
        public string text { get; set; }
    }
}
