using System;

namespace AutoFixture.Idioms
{
    public class WhitespaceOnlyStringBehaviorExpectation : IBehaviorExpectation
    {
        public void Verify(IGuardClauseCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (command.RequestedType != typeof(string))
                return;

            try
            {
                command.Execute(" ");
            }
            catch (ArgumentException)
            {
                return;
            }
            catch (Exception e)
            {
                throw command.CreateException("\" \"", e);
            }

            throw command.CreateException("\" \"");
        }
    }
}