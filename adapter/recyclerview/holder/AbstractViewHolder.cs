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

using Android.Support.V7.Widget;


namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder
{
    /// <summary>Created by evrencoskun on 23/10/2017.</summary>
    public abstract class AbstractViewHolder : RecyclerView.ViewHolder
    {
        public enum SelectionState
        {
            Selected,
            Unselected,
            Shadowed
        }

        private AbstractViewHolder.SelectionState m_eState = AbstractViewHolder.SelectionState.Unselected;

        public AbstractViewHolder(Android.Views.View itemView) : base(itemView)
        {
        }

        // Default value
        public virtual void SetSelected(AbstractViewHolder.SelectionState selectionState)
        {
            m_eState = selectionState;
            if (selectionState == AbstractViewHolder.SelectionState.Selected)
            {
                ItemView.Selected = true;
            }
            else
            {
                if (selectionState == AbstractViewHolder.SelectionState.Unselected)
                {
                    ItemView.Selected = false;
                }
            }
        }

        public bool Selected => IsSelected();

        public virtual bool IsSelected()
        {
            return m_eState == AbstractViewHolder.SelectionState.Selected;
        }

        public bool Shadowed => IsShadowed();

        public virtual bool IsShadowed()
        {
            return m_eState == AbstractViewHolder.SelectionState.Shadowed;
        }

        public virtual void SetBackgroundColor(int p_nColor)
        {
            ItemView.SetBackgroundColor(new Android.Graphics.Color(p_nColor));
        }

        public virtual void OnViewRecycled()
        {
        }

        public virtual bool OnFailedToRecycleView()
        {
            return false;
        }
    }
}
