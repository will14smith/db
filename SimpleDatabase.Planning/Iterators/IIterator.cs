using SimpleDatabase.Execution;

namespace SimpleDatabase.Planning.Iterators
{
    /// <summary>
    /// Used by
    ///     Init()
    /// s:  MoveNext(s, f)
    ///     Output.Load()
    ///     Yield (depends on output)
    ///     J s
    /// f:  Finished
    /// </summary>
    public interface IIterator
    {
        /// <summary>
        /// Describes the output of the iterator
        /// </summary>
        IteratorOutput Output { get; }

        /// <summary>
        /// Sets up the iterator
        /// When the iterator is empty it should jump to the label
        /// </summary>
        void GenerateInit();
        /// <summary>
        /// Moves to the next item in the iterator
        /// If there are more items it should jump to the label
        /// </summary>
        void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd);
        /// <summary>
        /// Moves the iterator back the initial state
        /// </summary>
        void Reset();
    }
}