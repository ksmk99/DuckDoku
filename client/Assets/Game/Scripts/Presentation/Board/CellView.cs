using System;
using DuckDoku.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Image _background;                                                 
        [SerializeField] private Image _content;     
        [SerializeField] private Image _conflictFrame;
        [SerializeField] private Button _button;                                                    
                                                                                              
        [SerializeField] private Sprite _crossSprite;                                               
        [SerializeField] private Sprite _duckSprite;

        [SerializeField] private GameObject _borderTop;
        [SerializeField] private GameObject _borderBottom;
        [SerializeField] private GameObject _borderLeft;
        [SerializeField] private GameObject _borderRight;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _conflictColor = Color.red;

        private Action _clicked;

        public void Setup(Color regionColor, Action clicked)
        {
            _background.color = regionColor;
            _clicked = clicked;
        }

        public void SetBorders(bool top, bool bottom, bool left, bool right)
        {
            _borderTop.SetActive(top);
            _borderBottom.SetActive(bottom);
            _borderLeft.SetActive(left);
            _borderRight.SetActive(right);
        }

        public void Show(CellState state, bool hasConflict)                                         
        {                                                                                           
            switch (state)                                                                          
            {                                                                                       
                case CellState.Cross:                                                               
                    _content.enabled = true;                                                        
                    _content.sprite = _crossSprite;                                                 
                    break;                                                                          
                                                                                              
                case CellState.Duck:                                                                
                    _content.enabled = true;                                                        
                    _content.sprite = _duckSprite;                                                  
                    break;                                                                          
                                                                                              
                default:                                                                            
                    _content.enabled = false;                                                       
                    break;                                                                          
            }                                                                                       
                                                                                              
            _conflictFrame.color = hasConflict ? _conflictColor : _normalColor;                           
        }
        
        public void Release()                                                                
        {                                                                                    
            _clicked = null;                                                                 
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _clicked?.Invoke();
        }
    }
}