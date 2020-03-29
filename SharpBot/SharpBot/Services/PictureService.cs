using Newtonsoft.Json;
using System;
using System.IO;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpBot.Services {
    public class PictureService {

        private readonly HttpClient _http;

        public PictureService(HttpClient http) {
            this._http = http;
        }

        public async Task<Stream> GetCatPictureAsync() {
            var response = await _http.GetAsync("https://cataas.com/cat");
            if (response == null || !response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> getRandomMeme() {
            var response = await _http.GetAsync("https://some-random-api.ml/img/dog");
            if (response == null || !response.IsSuccessStatusCode) return null;

            string imageUrl = JsonValue.Parse(await response.Content.ReadAsStringAsync())["link"];

            var image = await _http.GetAsync(imageUrl);
            if (image == null || !image.IsSuccessStatusCode) return null;

            return await image.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetRandomDogPicture() {
            var response = await _http.GetAsync("https://dog.ceo/api/breeds/image/random");
            if (response == null || !response.IsSuccessStatusCode) return null;

            string imageUrl = JsonValue.Parse(await response.Content.ReadAsStringAsync())["message"];

            var image = await _http.GetAsync(imageUrl);
            if (image == null || !image.IsSuccessStatusCode) return null;

            return await image.Content.ReadAsStreamAsync();
        }
    }
}
