﻿namespace Param_RootNamespace.Models
{
    public class TextImageResponse
    {
        public float textAngle { get; set; }
        public string orientation { get; set; }
        public string language { get; set; }
        public Region[] regions { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public Line[] lines { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public Word[] words { get; set; }
    }

    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }
}