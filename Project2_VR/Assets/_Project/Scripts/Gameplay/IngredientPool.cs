using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaratieBlade.Gameplay
{
    public class IngredientPool
    {
        private readonly Ingredient prefab;
        private readonly Transform container;
        private readonly Queue<Ingredient> available = new Queue<Ingredient>();
        private readonly Action<Ingredient> onCreated;

        public IngredientPool(Ingredient prefab, Transform container, Action<Ingredient> onCreated)
        {
            this.prefab = prefab;
            this.container = container;
            this.onCreated = onCreated;
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                available.Enqueue(Create());
            }
        }

        public Ingredient Get()
        {
            return available.Count > 0 ? available.Dequeue() : Create();
        }

        public void Return(Ingredient ingredient)
        {
            ingredient.gameObject.SetActive(false);
            available.Enqueue(ingredient);
        }

        private Ingredient Create()
        {
            Ingredient instance = Object.Instantiate(prefab, container);
            instance.gameObject.SetActive(false);
            onCreated?.Invoke(instance);
            return instance;
        }
    }
}
