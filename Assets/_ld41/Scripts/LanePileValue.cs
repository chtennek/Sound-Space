using UnityEngine.UI;

public class LanePileValue : GameValue
{
    public Image[] m_canvasDisplays;

    protected override float FillAmount
    {
        get
        {
            for (int i = 0; i < m_canvasDisplays.Length; i++)
            {
                if (i == m_canvasDisplays.Length - 1 || m_canvasDisplays[i].fillAmount != 1)
                    return 1 - (i + m_canvasDisplays[i].fillAmount) / m_canvasDisplays.Length;
            }
            return base.FillAmount;
        }

        set
        {
            float filled = (1 - value) * m_canvasDisplays.Length;
            for (int i = 0; i < m_canvasDisplays.Length; i++)
            {
                if (filled >= i + 1)
                    m_canvasDisplays[i].fillAmount = 1;
                else if (filled <= i)
                    m_canvasDisplays[i].fillAmount = 0;
                else
                    m_canvasDisplays[i].fillAmount = filled % 1;
            }
        }
    }
}
