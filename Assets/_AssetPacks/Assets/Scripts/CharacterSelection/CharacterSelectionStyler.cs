using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.View;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterSelection
{
    public interface ICharacterSelectionStyler
    {
        public ICharacterSelectionController CreateSelectionHome(CharacterSelectionStyles style, RectTransform parent);

        public ITagSelectionController CreateTagView(TagSelectionStyles style,  RectTransform parent);

        public IPlayerInformationController CreatePlayerInformationView(PlayerInformationInputStyles style,  RectTransform parent);
    }

    public class CharacterSelectionStyler : MonoBehaviour, ICharacterSelectionStyler
    {
        [SerializeField] private CharacterSelectionView characterSelectionViewPrefab;
        public ICharacterSelectionController CreateSelectionHome(CharacterSelectionStyles style, RectTransform parent)
        {
            return CharacterSelectionView.Factory(characterSelectionViewPrefab, parent);
        }
        [SerializeField] private TagSelectionView tagSelectionViewPrefab;
        public ITagSelectionController CreateTagView(TagSelectionStyles style, RectTransform parent)
        {
            return TagSelectionView.Factory(tagSelectionViewPrefab, parent);
        }
        [SerializeField] private PlayerInformationView playerInformationViewPrefab;
        public IPlayerInformationController CreatePlayerInformationView(PlayerInformationInputStyles style,  RectTransform parent)
        {
            return PlayerInformationView.Factory(playerInformationViewPrefab, parent);
        }
    }
}