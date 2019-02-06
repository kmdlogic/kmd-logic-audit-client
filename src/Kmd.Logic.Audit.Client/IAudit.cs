namespace Kmd.Logic.Audit.Client
{
    /// <summary>
    /// The core auditing interface. To write your own audit events, simply describe it
    /// as a sentence such as:
    /// <code>audit.ForContext("area", "security").Write("The user {UserId} logged in", userId)</code>
    /// </summary>
    /// <remarks>
    /// See https://messagetemplates.org/ and https://github.com/serilog/serilog/wiki/Writing-Log-Events for
    /// an introduction to message templates and how to write events.
    /// </remarks>
    public interface IAudit
    {
        /// <summary>
        /// Writes an audit event.
        /// </summary>
        /// <param name="messageTemplate">The event message template, including any named {Property} holes</param>
        /// <param name="propertyValues">The values to be substituted into the message template for the event</param>
        /// <remarks>
        /// See https://messagetemplates.org/ and https://github.com/serilog/serilog/wiki/Writing-Log-Events for
        /// an introduction to message templates and how to write events.
        /// </remarks>
        void Write(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Creates a new <see cref="IAudit"/> which will include contextual properties in any
        /// event that is later written to it.
        /// </summary>
        /// <param name="propertyName">The contextual property name</param>
        /// <param name="value">The property value</param>
        /// <param name="captureObjectStructure">When true,
        /// recursively captures the structure (properties) of the value. Beware to only use this
        /// when you can be certain the object graph will not cause a CPU and/or event size disaster.</param>
        /// <returns>A new instance with the context property associated</returns>
        IAudit ForContext(
            string propertyName,
            object value,
            bool captureObjectStructure = false);
    }
}
