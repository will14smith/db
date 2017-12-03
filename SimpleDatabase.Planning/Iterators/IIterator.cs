using SimpleDatabase.Execution;

namespace SimpleDatabase.Planning.Iterators
{
    /// <summary>
    /// Used by
    ///     Init(f)
    /// s:  Output.Load()
    ///     Yield (depends on output)
    ///     MoveNext(s)
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
        void GenerateInit(ProgramLabel emptyTarget);
        /// <summary>
        /// Moves to the next item in the iterator
        /// If there are more items it should jump to the label
        /// </summary>
        void GenerateMoveNext(ProgramLabel loopStartTarget);
    }
}