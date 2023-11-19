namespace MinesServer.GameShit.GUI.Horb
{
    public readonly record struct Card(CardImageType ImageType, string ImageURI, string Text)
    {
        public static Card Skill(short skill, string text) => new(CardImageType.Skill, skill.ToString(), text);

        public static Card Item(short item, string text) => new(CardImageType.Item, item.ToString(), text);

        public static Card Clan(short clan, string text) => new(CardImageType.Clan, clan.ToString(), text);

        public static Card WebImage(string uri, int width, int height, string text) => new(CardImageType.WebImage, $"{width}#{height}#{uri.Replace(":", "%")}", text);

    }
}
