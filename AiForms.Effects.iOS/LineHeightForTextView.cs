﻿using System;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace AiForms.Effects.iOS
{
    public class LineHeightForTextView : ILineHeightEffect
    {
        private UITextView _nativeTextView;
        private Editor _formsEditor;
        private LineHeightManager _manager;
        private NSAttributedString _orgString;


        public LineHeightForTextView(UIView container, UIView control, Element element)
        {
            _nativeTextView = control as UITextView;
            _formsEditor = element as Editor;
            _manager = new LineHeightManager();
            //_nativeTextView.LayoutManager.Delegate = _manager;
            _orgString = _nativeTextView.AttributedText;
        }

        public void OnDetached()
        {
            _nativeTextView.LayoutManager.Delegate = null;


            _nativeTextView.Text = _formsEditor.Text;
            _nativeTextView.AttributedText = _orgString;
            _orgString = null;

            _manager.Dispose();
            _manager = null;
            _nativeTextView = null;
            _formsEditor = null;
        }

        public void Update()
        {
            var multiple = AlterLineHeight.GetMultiple(_formsEditor);
            var fontSize = (float)(_formsEditor).FontSize;
            var lineSpacing = (fontSize * multiple) - fontSize;
            _manager.LineSpacing = (float)lineSpacing;

            _nativeTextView.Text = _nativeTextView.Text;

            var text = _formsEditor.Text;
            if (text == null) {
                return;
            }

            var pStyle = new NSMutableParagraphStyle() {
                LineSpacing = (float)lineSpacing
            };
            var attrString = new NSMutableAttributedString(text);

            attrString.AddAttribute(UIStringAttributeKey.ParagraphStyle,
                                    pStyle,
                                    new NSRange(0, attrString.Length));

            attrString.AddAttribute(UIStringAttributeKey.Font, _nativeTextView.Font, new NSRange(0, attrString.Length));
            attrString.AddAttribute(UIStringAttributeKey.ForegroundColor, _nativeTextView.TextColor, new NSRange(0, attrString.Length));

            _nativeTextView.AttributedText = attrString;
        }

    }

    internal class LineHeightManager : NSLayoutManagerDelegate
    {
        public float LineSpacing { get; set; }

        public override nfloat LineSpacingAfterGlyphAtIndex(NSLayoutManager layoutManager, nuint glyphIndex, CoreGraphics.CGRect rect)
        {
            return LineSpacing;
        }
    }
}
