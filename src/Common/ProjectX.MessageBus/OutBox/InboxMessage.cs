using System;

namespace ProjectX.MessageBus.Outbox
{
    /// <summary>
    /// The message is used to save base integration event information to DB to avoid handling the same event multiple times.
    /// </summary>
    //public class InboxMessage
    //{
    //    /// <summary>
    //    /// Integration event id.
    //    /// </summary>
    //    public Guid Id { get; set; }

    //    /// <summary>
    //    /// The property represents CLR type name of the integration event.
    //    /// </summary>
    //    public string MessageType { get; set; }

    //    /// <summary>
    //    /// The property represents the date-time when the message was handled.
    //    /// </summary>
    //    public DateTime ProcessedAt { get; set; }
    //}
}
