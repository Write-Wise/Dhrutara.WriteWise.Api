namespace Dhrutara.WriteWise.Providers.ContentProvider
{
    public static class Constants
    {
        private static Random random = new(0);
        public static Writer[] Poets { get; } = new Writer[] {
            new ("Vikram","Seth","English"),
            new ("Rita","Dove","English"),
            new ("Seamus","Heaney","English"),
            new ("June","Jordon","English"),
            new ("Sara","Teasdale","English"),
            new ("Fleur","Adcock","English"),
            new ("Margaret","Atwood","English"),
            new ("Ted","Hughes","English"),
            new ("Sylvia","Plath","English")
        };

        public static Writer[] JokeWriters { get; } = new Writer[] {
            new ("Dave","Barry","English"),
            new ("Stuart","McLean","English"),
            new ("Seamus","Heaney","English"),
            new ("Tim","Sample","English"),
            new ("Charles","Dickens","English"),
            new ("Oscar","Wilde","English"),
            new ("Tom","Sharpe","English"),
            new ("Douglas","Adams","English"),
            new ("Terry","Pratchett","English")
        };

        public static Writer[] DialogueWriters { get; } = new Writer[] {
            new ("Eric","Roth","English"),
            new ("Garth","Ennis","English"),
            new ("Hilary","Mantel","English"),
            new ("Gail","Simone","English"),
            new ("Lois","Lowry","English"),
            new ("Paul","Cornell","English"),
            new ("Andrew Kevin","Walker","English"),
            new ("Jim","Uhls","English"),
            new ("Reginald","Rose","English")
        };

        public static Writer GetARandomWriter(ContentType type)
        {
            switch (type)
            {
                case ContentType.Joke: 
                    return GetARandomItem(JokeWriters);
                case ContentType.Poem: 
                    return GetARandomItem(Poets);
                case ContentType.Message:
                default:
                    return GetARandomItem(DialogueWriters);
            }
        }

        private static Writer GetARandomItem(Writer[] writers)
        {
            int length = writers.Length;
            int index = random.Next(length - 1);
            return index < length 
                ? writers[index]
                : writers[0];
        }
    }

    public record Writer(string FirstName, string LastName, string language);
}