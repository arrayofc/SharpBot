using System;

namespace SharpBot {
    class Program {
        static void Main(string[] args) {
            new Sharp().MainAsync().GetAwaiter().GetResult();
        }
    }
}