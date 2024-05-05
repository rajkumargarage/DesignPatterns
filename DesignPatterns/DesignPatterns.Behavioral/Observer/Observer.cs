using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesignPatterns.Behavioral.ObserverPattern
{
    class Observer
    {
        public static void Invoke()
        {


            var weatherStation = new WeatherStation("ALL india weather station");

            var widget1 = new TemperatureWidget("Phone1 Widget", weatherStation);

            var widget2 = new TemperatureWidget("Phone2 Widget", weatherStation);

            weatherStation.Add(widget1);
            weatherStation.Add(widget2);

            weatherStation.UpdateTemperature();


            var arnold = new Person("Arnold");
            arnold.FallsIll += CallDaoctor;
            arnold.CatchACold();
            var btn = new Button();
            var formWindow = new Window("Sales Form", btn);
            var weakReferencene = new WeakReference(formWindow);
            formWindow.Save();

            Console.WriteLine($"{formWindow.Name} window is set to null");
            formWindow = null;


            FireGC();

            Console.WriteLine($"Is the { ((Window)weakReferencene.Target).Name} window alive?{weakReferencene.IsAlive}");

            var notifier = new Notifier();
            notifier.PropertyChanged += Notifier_PropertyChanged;
            notifier.NotificationMessages.ListChanged += NotificationMessages_ListChanged;
            notifier.Count += 1;
            notifier.NotificationMessages.Add("First Notification");
        }

        private static void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Notifier's {e.PropertyName} has beed updated");
        }

        private static void NotificationMessages_ListChanged(object sender, ListChangedEventArgs e)
        {
            Console.WriteLine("Notification message list got modified");
        }

        private static void FireGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static void CallDaoctor(object sender, PersonFallsIllEventArgs e)
        {
            Console.WriteLine($"Doctor will be arrive with in 10 mins on {e.Address}");
        }
    }



    interface ICustomObservable
    {
        void Add(ICustomObserver a);
        void Remove(ICustomObserver a);
        void Notify();
    }

    public interface ICustomObserver
    {
        void Update();
    }

    public interface IWeatherStation
    {
        string Name { get; }
        bool UpdateTemperature();
        decimal GetTemperature();
    }

    class WeatherStation : ICustomObservable, IWeatherStation
    {
        private List<ICustomObserver> observers { get; set; } = new List<ICustomObserver>();
        public string Name { get; }

        private decimal temperature;
        public WeatherStation(string name)
        {
            UpdateTemperature();
            Name = name;
        }
        public void Add(ICustomObserver a)
        {
            observers.Add(a);
        }

        public void Remove(ICustomObserver a)
        {
            observers.Remove(a);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }

        public decimal GetTemperature() => temperature;

        public bool UpdateTemperature()
        {
            temperature = new Random().Next(-50, 50);
            Task.Run(() => Notify());
            return true;
        }
    }

    class TemperatureWidget : ICustomObserver
    {
        private IWeatherStation weatherStation;

        public TemperatureWidget(string name, IWeatherStation weatherStation)
        {
            Name = name;
            this.weatherStation = weatherStation ?? throw new ArgumentNullException(nameof(weatherStation));
        }

        public string Name { get; }

        public void Update()
        {
            Console.WriteLine($"Widget:{Name}   Current Temperature:{this.weatherStation.GetTemperature()}");
        }
    }



    class PersonFallsIllEventArgs : EventArgs
    {
        public string Address { get; set; }
    }
    class Person
    {
        public Person(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }


        public void CatchACold()
        {
            FallsIll?.Invoke(this, new PersonFallsIllEventArgs() { Address = "7G Rainbow colony,Chennai." });
        }

        public event EventHandler<PersonFallsIllEventArgs> FallsIll;
    }

    interface IButton
    {
        void Fire(object sender, EventArgs eventArgs);
    }

    class Button : IButton
    {

        public event EventHandler<EventArgs> Clicked;

        public void Fire(object sender, EventArgs eventArgs)
        {
            Clicked?.Invoke(sender, eventArgs);
        }
    }

    class Window
    {
        Button saveButton;
        public string Name { get; private set; }
        public Window(string name, Button saveButton)
        {
            this.saveButton = saveButton;
            saveButton.Clicked += SaveButton_Clicked;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));

            Console.WriteLine($"{Name} Window Created");
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine($"Save button of {Name} window fired");
        }

        internal void Save()
        {
            saveButton.Fire(this, EventArgs.Empty);
        }

        ~Window()
        {
            Console.WriteLine($"{Name} Window Deleted");
        }
    }
    class SpecialWindow
    {
        private Button saveButton;
        public string Name { get; private set; }
        public SpecialWindow(string name, Button saveButton)
        {
            // WeakEventManager 
            this.saveButton = saveButton;
            saveButton.Clicked += SaveButton_Clicked;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));

            Console.WriteLine($"{Name} Window Created");
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine($"Save button of {Name} window fired");
        }

        internal void Save()
        {
            saveButton.Fire(this, EventArgs.Empty);
        }


        ~SpecialWindow()
        {
            Console.WriteLine($"{Name} Window Deleted");
        }
    }


    class Notifier : INotifyPropertyChanged
    {
        private int count;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get => count;

            set { count = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count))); }
        }

        public BindingList<string> NotificationMessages { get; set; } = new BindingList<string>();
    }

}
