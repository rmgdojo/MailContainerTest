namespace MailContainerTest.Types
{
    [Flags]
    public enum MailType : short
    {
        StandardLetter = 1,
        LargeLetter = 2,
        SmallParcel = 4
    }
}
