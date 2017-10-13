using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using ProEntry.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Views.InputMethods;
using Android.Text;
using Java.Lang;

[assembly: ExportRenderer(typeof(ProEntry.Gui.NativeEntry), typeof(NativeEntryRenderer))]
namespace ProEntry.Droid.Renderers {


    public class NativeEntry : EditText {

        public event EventHandler OnEnterPressed;
        public new event EventHandler TextChanged;

        public NativeEntry(Context context) : base(context) {
        }

        public override bool OnKeyPreIme([GeneratedEnum] Keycode keyCode, KeyEvent e) {
            //System.Diagnostics.Debug.WriteLine("OnKeyPreIme");
            if (keyCode == Keycode.Enter && e.RepeatCount == 0 && e.Action == KeyEventActions.Down) {
                OnEnterPressed?.Invoke(this, null);
            }

            return base.OnKeyPreIme(keyCode, e);
        }

        protected override void OnTextChanged(ICharSequence text, int start, int lengthBefore, int lengthAfter) {
            TextChanged?.Invoke(this, null);
            SetSelection(lengthAfter);
            base.OnTextChanged(text, start, lengthBefore, lengthAfter);
        }
    }

    public class NativeEntryRenderer : ViewRenderer<ProEntry.Gui.NativeEntry, NativeEntry>, TextView.IOnEditorActionListener, ITextWatcher {
        NativeEntry nativeEntry;

        protected override void OnElementChanged(ElementChangedEventArgs<ProEntry.Gui.NativeEntry> e) {
            base.OnElementChanged(e);

            if (e.OldElement == null) {
                nativeEntry = new NativeEntry(Context);
                nativeEntry.AddTextChangedListener(this);
                nativeEntry.SetOnEditorActionListener(this);

                //nativeEntry.OnEnterPressed += NativeEntry_OnEnterPressed;
                //nativeEntry.TextChanged += NativeEntry_TextChanged;
                //nativeEntry.Text = Element.Text;
                nativeEntry.Hint = Element.Placeholder;
                nativeEntry.Text = Element.Text;

                nativeEntry.InputType = InputTypes.TextFlagNoSuggestions;
                //nativeEntry.ImeOptions = ImeAction.Go;
                //nativeEntry.SetSingleLine(true);
                SetNativeControl(nativeEntry);
            }


        }

        //private void NativeEntry_TextChanged(object sender, EventArgs e) {
        //    Element.Text = nativeEntry.Text;
        //}

        //private void NativeEntry_OnEnterPressed(object sender, EventArgs e) {
        //    ((IEntryController)Element).SendCompleted();
        //}

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == Entry.TextProperty.PropertyName) {
                if (Control != null) {
                    if (Control.Text != Element.Text) {
                        Control.Text = Element.Text;
                        if (Control.IsFocused) {
                            Control.SetSelection(Control.Text.Length);
                        }
                    }
                }
            }

            base.OnElementPropertyChanged(sender, e);
        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e) {
            System.Diagnostics.Debug.WriteLine("OnEditorAction");
            if (actionId == ImeAction.Done || (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter)) {
                ((IEntryController)Element).SendCompleted();
            }

            return true;
        }

        public void AfterTextChanged(IEditable s) {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after) {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count) {
            if (string.IsNullOrEmpty(Element.Text) && s.Length() == 0) {
                return;
            }

            ((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
        }
    }

}

