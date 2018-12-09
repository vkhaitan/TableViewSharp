/*
* Copyright (c) 2018. Evren Coşkun
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*        http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*/

using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;


namespace Com.Evrencoskun.Tableview.Preference
{
    /// <summary>Created by evrencoskun on 4.03.2018.</summary>
    public class SavedState : View.BaseSavedState
    {
        public Preferences preferences;

        public SavedState(IParcelable superState) : base(superState)
        {
        }

        private SavedState(Parcel @in) : base(@in)
        {
            preferences = (Preferences) @in.ReadParcelable(Class.FromType(typeof(Preferences)).ClassLoader);
        }

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);
            dest.WriteParcelable(preferences, flags);
        }

        public sealed class SavedStateParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public SavedStateParcelableCreator()
            {
            }

            Java.Lang.Object IParcelableCreator.CreateFromParcel(Parcel source)
            {
                return new Com.Evrencoskun.Tableview.Preference.SavedState(source);
            }

            Java.Lang.Object[] IParcelableCreator.NewArray(int size)
            {
                return new Com.Evrencoskun.Tableview.Preference.SavedState[size];
            }
        }


        [Java.Interop.Export("CREATOR")]
        public new static SavedStateParcelableCreator Creator()
        {
            return new SavedStateParcelableCreator();
        }
    }
}
