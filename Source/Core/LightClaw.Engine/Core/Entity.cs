using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using NLog;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// A lightweight component base class implementing essential services such as <see cref="INotifyPropertyChanged"/>
    /// and <see cref="INameable"/>. Also provides a logger.
    /// </summary>
    [ThreadMode(true)]
    [DataContract(IsReference = true)]
    public abstract class Entity : INameable, INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies about changes in a specified property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Backing field.
        /// </summary>
        private Logger _Log;

        /// <summary>
        /// An instance of <see cref="ILog"/> used to track application events.
        /// </summary>
        [IgnoreDataMember]
        protected Logger Log
        {
            get
            {
                Contract.Ensures(Contract.Result<Logger>() != null);

                return _Log;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                _Log = value;
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private string _Name;

        /// <summary>
        /// The instance's name.
        /// </summary>
        [DataMember]
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                this.SetProperty(ref _Name, value);
            }
        }

        /// <summary>
        /// Backing field.
        /// </summary>
        private DryIoc.Container _IocC = LightClawEngine.DefaultIocContainer;

        /// <summary>
        /// An Ioc-Container used to obtain certain service instances such as IContentManager at runtime.
        /// </summary>
        [IgnoreDataMember]
        [CLSCompliant(false)]
        public DryIoc.Container IocC
        {
            get
            {
                Contract.Ensures(Contract.Result<DryIoc.Container>() != null);

                return _IocC;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _IocC, value);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="Entity"/>.
        /// </summary>
        protected Entity()
        {
            this.Log = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Initializes a new <see cref="Entity"/> and sets the name.
        /// </summary>
        /// <param name="name">The <see cref="Entity"/>s name.</param>
        protected Entity(string name)
            : this()
        {
            this.SetProperty(ref _Name, name, "Name");
        }

        /// <summary>
        /// Raises the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise.</param>
        /// <param name="args">Arguments for creation of a new <see cref="ParameterEventArgs"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Raise(EventHandler<ParameterEventArgs> handler, object args)
        {
            this.Raise(handler, new ParameterEventArgs(args));
        }

        /// <summary>
        /// Raises the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise.</param>
        /// <param name="args"><see cref="ParameterEventArgs"/> containing a parameter to be parsed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Raise(EventHandler<ParameterEventArgs> handler, ParameterEventArgs args = null)
        {
            if (handler != null)
            {
                this.Raise<ParameterEventArgs>(handler, args ?? ParameterEventArgs.Default);
            }
        }

        /// <summary>
        /// Raises the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise.</param>
        /// <param name="args"><see cref="ParameterEventArgs"/> containing a parameter to be parsed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Raise(EventHandler<UpdateEventArgs> handler, GameTime gameTime, int pass)
        {
            if (handler != null && (pass >= 0))
            {
                this.Raise(handler, new UpdateEventArgs(gameTime, pass));
            }
        }

        /// <summary>
        /// Raises the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise.</param>
        /// <param name="args">Arguments for creation of a new <see cref="ParameterEventArgs"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Raise<T>(EventHandler<T> handler, T args)
        {
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the specified <paramref name="handler"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of value that changed.</typeparam>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise.</param>
        /// <param name="newValue">The value of the variable after the change.</param>
        /// <param name="oldValue">The value of the variable before the change.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Raise<T>(EventHandler<ValueChangedEventArgs<T>> handler, T newValue, T oldValue)
        {
            if (handler != null)
            {
                this.Raise(handler, new ValueChangedEventArgs<T>(newValue, oldValue));
            }
        }

        /// <summary>
        /// Invokes the specified delegate via late binding. See remarks.
        /// </summary>
        /// <remarks>Late binding is slow, avoid this method wherever possible.</remarks>
        /// <param name="del">The <see cref="Delegate"/> to invoke.</param>
        /// <param name="parameters">Delegate parameters.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseDynamic(Delegate del, params object[] parameters)
        {
            Contract.Requires<ArgumentNullException>(parameters != null);

            if (del != null)
            {
                del.DynamicInvoke(parameters);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/>-event for the specified property name.
        /// </summary>
        /// <param name="propertyName">
        /// The property name that changed. Leave this blank, it will be filled out by the compiler.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(this, this.PropertyChanged, propertyName);
        }

        /// <summary>
        /// Sets the property with the specified name and raises the <see cref="E:PropertyChanged"/>-event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the property that changed.</typeparam>
        /// <param name="location">The property's backing field.</param>
        /// <param name="newValue">The property's new value.</param>
        /// <param name="propertyName">
        /// The property name that changed. Leave this blank, it will be filled out by the compiler.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetProperty<T>(ref T location, T newValue, [CallerMemberName] string propertyName = null)
        {
            SetProperty(this, this.PropertyChanged, ref location, newValue, propertyName);
        }

        /// <summary>
        /// Sets the property with the specified name and raises the <see cref="E:PropertyChanged"/>-event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the property that changed.</typeparam>
        /// <param name="location">The property's backing field.</param>
        /// <param name="newValue">The property's new value.</param>
        /// <param name="valueChangedHandler">An event handler to raise during the change operation.</param>
        /// <param name="propertyName">
        /// The property name that changed. Leave this blank, it will be filled out by the compiler.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetProperty<T>(ref T location, T newValue, EventHandler<ValueChangedEventArgs<T>> valueChangedHandler, [CallerMemberName] string propertyName = null)
        {
            T previous = location;
            SetProperty(ref location, newValue, propertyName);
            this.Raise(valueChangedHandler, newValue, previous);
        }

        /// <summary>
        /// Callback called during deserialization with data contract serializers.
        /// </summary>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Sets <see cref="P:IocC"/> to a non-null-value during deserialization.
        /// </summary>
        private void Initialize()
        {
            this.IocC = LightClawEngine.DefaultIocContainer;
        }

        /// <summary>
        /// Contains Contract.Invariant definitions.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._IocC != null);
            Contract.Invariant(this._Log != null);
        }

        /// <summary>
        /// Raises the property changed event on the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="sender">The object whose property was changed.</param>
        /// <param name="handler">The <see cref="PropertyChangedEventHandler"/> to raise.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RaisePropertyChanged(object sender, PropertyChangedEventHandler handler, [CallerMemberName] string propertyName = null)
        {
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Sets the property to the new value and raises the property changed event.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the property to set.</typeparam>
        /// <param name="sender">The object whose property was changed.</param>
        /// <param name="handler">The <see cref="PropertyChangedEventHandler"/> to raise.</param>
        /// <param name="location">The backing field of the property to set.</param>
        /// <param name="newValue">The properties new value.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetProperty<T>(object sender, PropertyChangedEventHandler handler, ref T location, T newValue, [CallerMemberName] string propertyName = null)
        {
            location = newValue;
            RaisePropertyChanged(sender, handler, propertyName);
        }
    }
}
