using System.IO;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SharpBot.Utility;

namespace SharpBot.Services {
    public class ImageWebService {

        private readonly HttpClient _http;

        public ImageWebService(HttpClient http) {
            _http = http;
        }

        /// <summary>
        /// Retrieve an image of specified image type.
        /// </summary>
        /// <param name="type">The type of image to retrieve.</param>
        /// <returns>The response stream.</returns>
        public async Task<Stream> RequestImage(ImageType type) {
            var url = type.GetUrlAttribute();

            var response = await _http.GetAsync(url);
            if (response == null || !response.IsSuccessStatusCode) return null;

            if (type == ImageType.Dog) {
                string imageUrl = JsonValue.Parse(await response.Content.ReadAsStringAsync())["message"];
            
                var image = await _http.GetAsync(imageUrl);
                if (image == null || !image.IsSuccessStatusCode) return null;

                return await image.Content.ReadAsStreamAsync();
            }

            return await response.Content.ReadAsStreamAsync();
        } 
    }
}