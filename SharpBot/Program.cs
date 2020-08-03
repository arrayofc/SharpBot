namespace SharpBot {
    internal static class Program {
        public static void Main(string[] args) {
            new Sharp().MainAsync().GetAwaiter().GetResult();
        }
    }
}