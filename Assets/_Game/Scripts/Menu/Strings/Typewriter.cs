using System.Collections;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour {
    private TMP_Text textMeshPro;

    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 0.2f;

	private void Start() {
		textMeshPro = GetComponent<TMP_Text>();
	}

    public void ShowText() {
        StartCoroutine(PrintRoutine(textMeshPro.text));
    }

    private IEnumerator PrintRoutine(string text) {
        textMeshPro.text = text;
        textMeshPro.maxVisibleCharacters = 0;

        for (int i = 0; i <= text.Length; i++) {
            textMeshPro.maxVisibleCharacters = i;

			if (i < text.Length && text[i] == ' ')
            continue;

            yield return new WaitForSeconds(Random.Range(minSpeed, maxSpeed));
        }
    }
}
