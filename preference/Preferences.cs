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

using Android.OS;
using Android.Runtime;
using Java.Lang;


namespace Com.Evrencoskun.Tableview.Preference
{
    /// <summary>Created by evrencoskun on 4.03.2018.</summary>
    public class Preferences : Java.Lang.Object, IParcelable
    {
        public int rowPosition;

        public int rowPositionOffset;

        public int columnPosition;

        public int columnPositionOffset;

        public int selectedRowPosition;

        public int selectedColumnPosition;

        public Preferences()
        {
        }

        protected internal Preferences(Parcel @in)
        {
            rowPosition = @in.ReadInt();
            rowPositionOffset = @in.ReadInt();
            columnPosition = @in.ReadInt();
            columnPositionOffset = @in.ReadInt();
            selectedRowPosition = @in.ReadInt();
            selectedColumnPosition = @in.ReadInt();
        }

        public sealed class PreferencesCreator : Java.Lang.Object, IParcelableCreator
        {
            public PreferencesCreator()
            {
            }

            Object IParcelableCreator.CreateFromParcel(Parcel source)
            {
                return new Com.Evrencoskun.Tableview.Preference.Preferences(source);
            }

            Object[] IParcelableCreator.NewArray(int size)
            {
                return new Com.Evrencoskun.Tableview.Preference.Preferences[size];
            }
        }

        [Java.Interop.ExportField("CREATOR")]
        public static PreferencesCreator Creator()
        {
            return new PreferencesCreator();
        }

        /// <summary>
        /// Describe the kinds of special objects contained in this Parcelable
        /// instance's marshaled representation.
        /// </summary>
        /// <remarks>
        /// Describe the kinds of special objects contained in this Parcelable
        /// instance's marshaled representation. For example, if the object will
        /// include a file descriptor in the output of
        /// <see cref="WriteToParcel(Android.OS.Parcel, int)"/>
        /// ,
        /// the return value of this method must include the
        /// <see cref="Android.OS.Parcelable.ContentsFileDescriptor"/>
        /// bit.
        /// </remarks>
        /// <returns>
        /// a bitmask indicating the set of special object types marshaled by this Parcelable
        /// object instance.
        /// </returns>
        public int DescribeContents()
        {
            return 0;
        }

        /// <summary>Flatten this object in to a Parcel.</summary>
        /// <param name="dest">The Parcel in which the object should be written.</param>
        /// <param name="flags">
        /// Additional flags about how the object should be written. May be 0 or
        /// <see cref="Android.OS.Parcelable.ParcelableWriteReturnValue"/>
        /// .
        /// </param>
        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteInt(rowPosition);
            dest.WriteInt(rowPositionOffset);
            dest.WriteInt(columnPosition);
            dest.WriteInt(columnPositionOffset);
            dest.WriteInt(selectedRowPosition);
            dest.WriteInt(selectedColumnPosition);
        }
    }
}
