using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.StatePattern
{
    public class State
    {
        public static void Invoke()
        {
            var tollGate = new Gate();
            tollGate.Enter();
            tollGate.PayemntSuccess();
            tollGate.Enter();
            tollGate.PayemntSuccess();
            tollGate.PayemntSuccess();

            //var newphone = new LandLinePhone("12345");
            //newphone.Unlock();
            //newphone.InitiateCall();

            var mobilePhone = new MobilePhone("qwerty");
            mobilePhone.OpenPhotos();
            mobilePhone.Unlock();
            mobilePhone.OpenPhotos();
        }
    }

    interface IStatefullGate
    {
        void Handle(GateState s);
    }

    abstract class GateState
    {
        protected Gate gate;

        protected GateState(Gate gate)
        {
            this.gate = gate ?? throw new ArgumentNullException(nameof(gate));
        }
        public abstract void Enter();
        public abstract void PayemntSuccess();
        public abstract void PaymentFailed();
    }

    class Gate : IStatefullGate
    {
        private GateState gateState;

        public Gate()
        {
            gateState = new CloseGateState(this);
        }

        public void Enter()
        {
            gateState.Enter();
        }

        public void Handle(GateState s)
        {
            gateState = s;
        }

        /// <summary>
        /// just delegating to gatestate need not to be a GateState. For understanding given same name as gatestate
        /// </summary>
        public void PayemntSuccess()
        {
            gateState.PayemntSuccess();

        }

        public void PaymentFailed()
        {
            gateState.PaymentFailed();
        }
    }

    class OpenGateState : GateState
    {
        public OpenGateState(Gate gate) : base(gate)
        {

        }

        public override void Enter()
        {
            Console.WriteLine("Current State:Opened   Message:Entering the opened gate");
            this.gate.Handle(this);
        }


        public override void PayemntSuccess()
        {
            Console.WriteLine("Current State:Opened   Message:Gate is opened already.Money is not deducted");
            this.gate.Handle(this);
        }

        public override void PaymentFailed()
        {
            Console.WriteLine("Current State:Closed   Message:Payment failed on opened gate. Closing the gate");
            this.gate.Handle(new CloseGateState(gate));
        }
    }

    class CloseGateState : GateState
    {
        public CloseGateState(Gate gate) : base(gate)
        {

        }

        public override void Enter()
        {
            Console.WriteLine("Current State:Closed   Message:Please make a payment to enter");
            this.gate.Handle(this);
        }

        public override void PayemntSuccess()
        {
            Console.WriteLine("Current State:Opened   Message:Payment success opening the gate");
            this.gate.Handle(new OpenGateState(gate));
        }

        public override void PaymentFailed()
        {
            Console.WriteLine("Current State:Closed   Message:Payment failed.Please make a payment to enter. Gate remains closed.");
            this.gate.Handle(this);
        }
    }


    enum PhoneState
    {
        OffHook,
        Connecting,
        Connected,
        OnHold,
        PlacedOnHook
    }

    enum UserAction
    {
        DailNewCall,
        HangUp,
        WaitForResponse,
        Hold,
        UnHold,
        LeaveMessage,
        Exit
    }

    enum PhoneLockState
    {
        Locked,
        Unlocked,
        Failed
    }

    class LandLinePhone
    {
        static Dictionary<PhoneState, List<(UserAction, PhoneState)>> rules;
        private string Password { get; }

        public LandLinePhone(string password)
        {
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        static LandLinePhone()
        {
            rules = new Dictionary<PhoneState, List<(UserAction, PhoneState)>>()
            {
                [PhoneState.OffHook] = new List<(UserAction, PhoneState)>
                {
                    (UserAction.DailNewCall,PhoneState.Connecting),
                    (UserAction.Exit,PhoneState.PlacedOnHook),
                },
                [PhoneState.Connecting] = new List<(UserAction, PhoneState)>
                {
                    (UserAction.WaitForResponse ,PhoneState.Connected),
                    (UserAction.LeaveMessage ,PhoneState.OffHook),
                    (UserAction.HangUp ,PhoneState.OffHook)
                },
                [PhoneState.Connected] = new List<(UserAction, PhoneState)>
                {
                    (UserAction.Hold ,PhoneState.OnHold),
                    (UserAction.HangUp ,PhoneState.OffHook)
                },
                [PhoneState.OnHold] = new List<(UserAction, PhoneState)>
                {
                    (UserAction.UnHold ,PhoneState.Connected),
                    (UserAction.HangUp ,PhoneState.OffHook)
                },
                [PhoneState.PlacedOnHook] = new List<(UserAction, PhoneState)>
                {

                }
            };
        }

        public void Unlock()
        {
            Console.WriteLine("Please enter password to unlock the phone");
            var phoneLockState = PhoneLockState.Locked;
            var pass = new StringBuilder();
            while (true)
            {
                switch (phoneLockState)
                {
                    case PhoneLockState.Locked:
                        {
                            var input = Console.ReadKey();
                            if (input.Key == ConsoleKey.Backspace || input.Key == ConsoleKey.Delete)
                            {
                                break;
                            }
                            if (pass.Append(input.KeyChar).ToString() == Password)
                            {
                                phoneLockState = PhoneLockState.Unlocked;
                            }

                            if (!Password.StartsWith(pass.ToString()))
                            {
                                phoneLockState = PhoneLockState.Failed;
                            }
                            break;
                        }
                    case PhoneLockState.Failed:
                        {
                            Console.CursorLeft = 0;
                            Console.WriteLine("Failed");
                            pass.Clear();
                            phoneLockState = PhoneLockState.Locked;
                            goto case PhoneLockState.Locked;
                        }
                    case PhoneLockState.Unlocked:
                        {
                            Console.CursorLeft = 0;
                            Console.WriteLine("Unlocked");
                            return;
                        }
                }
            }

        }

        public void InitiateCall()
        {
            var phoneState = PhoneState.OffHook;
            while (true)
            {
                Console.WriteLine($"The Phone is currently {phoneState}");
                if (rules[phoneState].Count == 0)
                    break;
                else
                    Console.WriteLine("Select a option");

                for (int i = 0; i < rules[phoneState].Count; i++)
                {
                    var (tempCallState, _) = rules[phoneState][i];
                    Console.WriteLine($"{i}.{tempCallState}");
                }


                while (rules[phoneState].Count > 0 && true)
                {
                    if (!int.TryParse(Console.ReadLine(), out int selectedOption) || selectedOption >= rules[phoneState].Count)
                        Console.WriteLine("Please Enter valid option");
                    else
                    {
                        phoneState = (rules[phoneState][selectedOption]).Item2;
                        break;
                    }
                }
            }
        }
    }

    abstract class BasePhoneLockState
    {
        protected MobilePhone mobilePhone;

        protected BasePhoneLockState(MobilePhone mobilePhone)
        {
            this.mobilePhone = mobilePhone ?? throw new ArgumentNullException(nameof(mobilePhone));
        }

        abstract public void Success();
        abstract public void Fail(int numberOfAttempts);

    }

    class LockedPhoneState : BasePhoneLockState
    {
        public LockedPhoneState(MobilePhone mobilePhone) : base(mobilePhone)
        {
            Log();
        }

        public override void Fail(int numberOfAttempts)
        {
            if (numberOfAttempts > 2)
                mobilePhone.SetLockState(new FailedPhoneState(mobilePhone));
            else
            {
                Log();
                mobilePhone.SetLockState(this);
            }
        }

        public override void Success()
        {
            mobilePhone.SetLockState(new UnlockedPhoneState(mobilePhone));
        }
        private void Log()
        {
            Console.WriteLine("State:Locked Message:Password not matched");
        }
    }

    class UnlockedPhoneState : BasePhoneLockState
    {
        
        public UnlockedPhoneState(MobilePhone mobilePhone) : base(mobilePhone)
        {
            Log();
        }

        public override void Fail(int numberOfAttempts)
        {
            if (numberOfAttempts > 2)

                mobilePhone.SetLockState(new FailedPhoneState(mobilePhone));
            else

                mobilePhone.SetLockState(new LockedPhoneState(mobilePhone));
        }


        public override void Success()
        {
            Console.WriteLine($"State:UnLocked Message:Already unlocked.");
            mobilePhone.SetLockState(this);
        }

        private void Log()
        {
            Console.WriteLine("State:Unlocked Message:Welcome to the digital world");
        }
    }

    class FailedPhoneState : BasePhoneLockState
    {
        public FailedPhoneState(MobilePhone mobilePhone) : base(mobilePhone)
        {
            Log();
        }

        public override void Fail(int numberOfAttempts)
        {
            if (numberOfAttempts > 2)
                mobilePhone.SetLockState(new FailedPhoneState(mobilePhone));
            else
                mobilePhone.SetLockState(new LockedPhoneState(mobilePhone));
        }

        public override void Success()
        {
            Console.WriteLine("Please enter OTP to proceed");
            Console.WriteLine("OTP Entered successully");
            mobilePhone.SetLockState(new UnlockedPhoneState(mobilePhone));
        }
        private void Log()
        {
            Console.WriteLine("State:Failed Message:Reached maximum number of try. Try after 1 minute");
        }
    }

    class MobilePhone
    {
        BasePhoneLockState basePhoneLockState;

        string Password = "12345";

        public MobilePhone(string password)
        {
            Password = password;
            SetLockState(new LockedPhoneState(this));
        }

        public void SetLockState(BasePhoneLockState basePhoneLockState)
        {
            this.basePhoneLockState = basePhoneLockState;
        }

        public void Unlock()
        {
            for (int i = 0; i < 4; i++)
            {
                var numberOfAttempts = i + 1;
                if (VerifyPassword())
                {
                    basePhoneLockState.Success();
                    break;
                }
                else
                {
                    basePhoneLockState.Fail(numberOfAttempts);
                }
            }
        }

        private bool VerifyPassword()
        {
            Console.WriteLine("Please enter password:");
            return Password == Console.ReadLine();
        }

        private bool IsPhoneUnlocked()
        {
            return basePhoneLockState is UnlockedPhoneState;
        }

        public void OpenPhotos()
        {
            if (!IsPhoneUnlocked())
            {
                Console.WriteLine("Please unlock the phone to view photos");
                return;
            }

            Console.WriteLine("Phot01.jpg");
        }
    }

}
