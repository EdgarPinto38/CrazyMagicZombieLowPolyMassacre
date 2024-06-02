using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class Timer : MonoBehaviourPunCallbacks, IPunObservable
{
     public float timeRemaining = 10f; // Tiempo inicial en segundos
    public TMP_Text timeText;
    private bool timerIsRunning = false;
    private bool matchEnded = false;

    [SerializeField] CanvasGroup canvasGroup;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timerIsRunning = true;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining < 0)
                {
                    timeRemaining = 0;
                }
            }
            else
            {
                Debug.Log("Time has run out!");
                timerIsRunning = false;
                // Aquí puedes agregar cualquier lógica adicional para cuando el tiempo se acaba
                EndMatch();
            }
        }
        UpdateTimeDisplay(timeRemaining);
    }

    void UpdateTimeDisplay(float timeToDisplay)
    {
        timeToDisplay += 1; // Ajusta para evitar mostrar 0 segundos cuando el temporizador llega a cero

        // Calcula minutos y segundos
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Actualiza el texto del temporizador
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // El MasterClient envía el tiempo restante a los demás jugadores
            stream.SendNext(timeRemaining);
        }
        else
        {
            // Los demás jugadores reciben el tiempo restante
            timeRemaining = (float)stream.ReceiveNext();
        }
    }

     void EndMatch()
    {
        matchEnded = true;
        photonView.RPC("MatchOver", RpcTarget.All);
    }

    [PunRPC]
    void MatchOver()
    {
        // Aquí puedes agregar cualquier lógica adicional para el final de la partida, como mostrar una pantalla de resultados
         canvasGroup.alpha = 1;
        // Volver al menú principal después de un breve retraso
        StartCoroutine(ReturnToMenu());
    }

    System.Collections.IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(6f); // Esperar 6 segundos antes de volver al menú
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu"); // Cargar la escena del menú principal
    }
}
