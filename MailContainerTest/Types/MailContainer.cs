namespace MailContainerTest.Types
{
    public class MailContainer
    {
        public string MailContainerNumber { get; set; } 
        public int Capacity { get; set; }   
        public MailContainerStatus Status { get; set; }
        //Thought: Should be able to take more that one type
        public List<AllowedMailType> AllowedMailType { get; set; }

    }
}
