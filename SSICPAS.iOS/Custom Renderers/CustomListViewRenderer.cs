using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CoreGraphics;
using Foundation;
using SSICPAS.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(CustomListViewRenderer))]
namespace SSICPAS.iOS.CustomRenderers
{
    // Fixes broken ListView header heights on iOS
    public class CustomListViewRenderer : ListViewRenderer
    {
        private static readonly FieldInfo fieldInfo_ListViewRenderer_dataSource = typeof(ListViewRenderer).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly Type type_ListViewRenderer_ListViewDataSource = typeof(ListViewRenderer).GetNestedType("ListViewDataSource", BindingFlags.NonPublic);
        private static readonly Type type_ListViewRenderer_UnevenListViewDataSource = typeof(ListViewRenderer).GetNestedType("UnevenListViewDataSource", BindingFlags.NonPublic);
        private static readonly ConstructorInfo ctorInfo_ListViewRenderer_ListViewDataSource = type_ListViewRenderer_ListViewDataSource.GetConstructor(new[] { fieldInfo_ListViewRenderer_dataSource.FieldType });
        private static readonly ConstructorInfo ctorInfo_ListViewRenderer_UnevenListViewDataSource = type_ListViewRenderer_UnevenListViewDataSource.GetConstructor(new[] { fieldInfo_ListViewRenderer_dataSource.FieldType });

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.Source = new CustomListViewDataSource(Control.Source);
            }

            if (Control != null)
            {
                Control.RowHeight = UITableView.AutomaticDimension;
                Control.EstimatedRowHeight = 40;
                Control.SectionHeaderHeight = UITableView.AutomaticDimension;
                Control.EstimatedSectionHeaderHeight = 40;
                Control.SectionFooterHeight = 0;
                Control.EstimatedSectionFooterHeight = 0;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ListView.HasUnevenRowsProperty.PropertyName)
            {
                ConstructorInfo ctor = Element.HasUnevenRows ? ctorInfo_ListViewRenderer_UnevenListViewDataSource : ctorInfo_ListViewRenderer_ListViewDataSource;
                object dataSource = fieldInfo_ListViewRenderer_dataSource.GetValue(this);
                UITableViewSource tvs = (UITableViewSource)ctor.Invoke(new[] { dataSource });
                fieldInfo_ListViewRenderer_dataSource.SetValue(this, tvs);
                Control.Source = new CustomListViewDataSource(tvs);
                Control.ReloadData();
            }
            else
            {
                base.OnElementPropertyChanged(sender, e);
            }
        }

        public class CustomListViewDataSource : UITableViewSource
        {
            private readonly UITableViewSource inner;
            private readonly MethodInfo methodInfo_DetermineEstimatedRowHeight;
            private PropertyInfo propertyInfo_HeaderWrapperView_Cell;

            public CustomListViewDataSource(UITableViewSource inner)
            {
                this.inner = inner;
                methodInfo_DetermineEstimatedRowHeight = inner.GetType().BaseType.GetMethod("DetermineEstimatedRowHeight", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                nfloat r = inner.GetHeightForHeader(tableView, section);
                r = UITableView.AutomaticDimension;
                return r;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                UIView innerWrapper = inner.GetViewForHeader(tableView, section);
                PropertyInfo pi = propertyInfo_HeaderWrapperView_Cell ?? (propertyInfo_HeaderWrapperView_Cell = innerWrapper.GetType().GetProperty("Cell", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                Cell cell = (Cell)pi.GetValue(innerWrapper);

                UIView cellView = innerWrapper.Subviews.LastOrDefault();
                HeaderWrapperView newWrapper = new HeaderWrapperView(cell, cellView);
                return newWrapper;
            }

            public override void HeaderViewDisplayingEnded(UITableView tableView, UIView headerView, nint section)
            {
                if (headerView is HeaderWrapperView wrapper)
                {
                    wrapper.Cell?.SendDisappearing();
                    wrapper.Cell = null;
                }
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                // Newer versions of XamForms override this method in UITableViewSource - so to maintain compat with older and newer, we use conditional reflection to pass-thru this method call.
                if (methodInfo_DetermineEstimatedRowHeight != null)
                    methodInfo_DetermineEstimatedRowHeight.Invoke(inner, new object[0]);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                return inner.GetCell(tableView, indexPath);
            }

            public override nint RowsInSection(UITableView tableView, nint section)
            {
                return inner.RowsInSection(tableView, section);
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return inner.GetHeightForRow(tableView, indexPath);
            }

            public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
            {
                inner.DraggingEnded(scrollView, willDecelerate);
            }

            public override void DraggingStarted(UIScrollView scrollView)
            {
                inner.DraggingStarted(scrollView);
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return inner.NumberOfSections(tableView);
            }

            public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
            {
                inner.RowDeselected(tableView, indexPath);
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                inner.RowSelected(tableView, indexPath);
            }

            public override void Scrolled(UIScrollView scrollView)
            {
                inner.Scrolled(scrollView);
            }

            public override string[] SectionIndexTitles(UITableView tableView)
            {
                return inner.SectionIndexTitles(tableView);
            }

            protected override void Dispose(bool disposing)
            {
                inner.Dispose();
            }

            private class HeaderWrapperView : UIView
            {
                public Cell Cell { get; set; }
                public UIView Subview { get; private set; }

                public HeaderWrapperView(Cell cell, UIView subview)
                {
                    Cell = cell;
                    Subview = subview;
                    AddSubview(subview);
                }

                public override CGSize SizeThatFits(CGSize size)
                {
                    return Subview.SizeThatFits(size);
                }

                public override CGSize IntrinsicContentSize => Subview.IntrinsicContentSize;

                public override void LayoutSubviews()
                {
                    base.LayoutSubviews();
                    Subview.Frame = Bounds;
                }
            }
        }
    }
}
