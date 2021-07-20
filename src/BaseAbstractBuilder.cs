namespace StateMachine {
    /// <summary>
    /// A generic extendable builder
    /// Return the "Self" variable to get the generic builder to return at each step
    /// </summary>
    /// <typeparam name="T">Object being built</typeparam>
    /// <typeparam name="TBuilder">The builder</typeparam>
    public abstract class BaseAbstractBuilder<T, TBuilder> where TBuilder : BaseAbstractBuilder<T, TBuilder>, new() {
        /// <summary>
        /// Generic builder
        /// </summary>
        protected readonly TBuilder Self;

        protected BaseAbstractBuilder() {
            Self = (TBuilder)this;
        }


        /// <summary>
        /// Builds the object
        /// </summary>
        /// <returns>Object</returns>
        public abstract T Build();
    }
}
