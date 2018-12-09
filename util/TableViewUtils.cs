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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;


namespace Com.Evrencoskun.Tableview.Util
{
    /// <summary>Created by evrencoskun on 18/09/2017.</summary>
    public static class TableViewUtils
    {
        /// <summary>Helps to force width value before calling requestLayout by the system.</summary>
        public static void SetWidth(Android.Views.View view, int width)
        {
            // Change width value from params
            ((RecyclerView.LayoutParams) view.LayoutParameters).Width = width;
            int widthMeasureSpec = View.MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly);
            int heightMeasureSpec = View.MeasureSpec.MakeMeasureSpec(view.MeasuredHeight, MeasureSpecMode.Exactly);
            view.Measure(widthMeasureSpec, heightMeasureSpec);
            view.RequestLayout();
        }

        /// <summary>Gets the exact width value before the view drawing by main thread.</summary>
        public static int GetWidth(Android.Views.View view)
        {
            view.Measure(LinearLayout.LayoutParams.WrapContent,
                View.MeasureSpec.MakeMeasureSpec(view.MeasuredHeight, MeasureSpecMode.Exactly));
            return view.MeasuredWidth;
        }

        public static void SafeFire<TEventArgs>(this EventHandler<TEventArgs> theEvent, object obj,
            TEventArgs theEventArgs) 
        {
            if (theEvent != null)
                theEvent(obj, theEventArgs);
        }
    }
}
