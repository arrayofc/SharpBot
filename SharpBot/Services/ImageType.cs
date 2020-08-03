using System;

namespace SharpBot.Services {
    internal class UrlAttr : Attribute {
        internal UrlAttr(string url) {
            Url = url;
        }
        
        public string Url { get; }
    }
    
    public enum ImageType {
        [UrlAttr("https://dog.ceo/api/breeds/image/random")]
        Dog,
        
        [UrlAttr("https://cataas.com/cat")]
        Cat
    }
}