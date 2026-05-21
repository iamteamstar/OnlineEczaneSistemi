Online Ezcane Sistemi

Amaç: hastalar eczaneye gidip reçete okutmak yerine kendisine yakın eczanelerden sipariş verebilir. Sisteme reçete ekran görüntüsünü, reçete kodunu, adresini, sipariş vermek istediği eczaneyi seçmesi kafii olacaktır :
<img width="1469" height="672" alt="image" src="https://github.com/user-attachments/assets/3d3a368f-5d1a-479c-be1f-4f36549f04c4" />
<img width="746" height="214" alt="image" src="https://github.com/user-attachments/assets/f1d85adc-973f-42dd-845a-0d39582cc9d0" />


Ezcaneler gelen siparişi görür, detayına iner ve reçetedeki ilaçları stoktan alır yani stok kontrol sistemi mevcuttur
<img width="1583" height="435" alt="image" src="https://github.com/user-attachments/assets/bb78e3db-0b49-4f9d-9a58-841ad11a6e55" />

Eczanelerin stok sistemi şu şekildedir:
<img width="1553" height="636" alt="image" src="https://github.com/user-attachments/assets/ce2d968f-686f-4794-b652-0474b18ec120" />

Ezcane siparişi alıp detaya indiğinde "Hazırlamaya başla" enumu çalışır. daha sonra gerekli ilaçları sisteme yükler

<img width="1396" height="726" alt="image" src="https://github.com/user-attachments/assets/6fe67c00-65a4-45cc-aee5-60cd53309f38" />

Tabi siparişin hazırlanmaya başladığı bilgisi kullanıcıya gitmektedir

<img width="557" height="425" alt="image" src="https://github.com/user-attachments/assets/5483654a-4408-47d7-82b7-bf8289648913" />

daha sonra eczane eğer "hazırlandı" enumuna geçerse kuryeye sipariş düşer 
<img width="623" height="367" alt="image" src="https://github.com/user-attachments/assets/79df687e-644a-45a4-85dc-54b37fa77cde" />
<img width="1492" height="676" alt="image" src="https://github.com/user-attachments/assets/f63bfe4c-c906-4c09-be49-6b8943a549a2" />

kurye teslim edildi enumuna geçer teslimattan sonra, daha sonra sistem geçmişe loglar bu süreci.

Admin kullanıcılar üzerinde değişiklikler ve birçok istatistikliğe hakimdir
<img width="1603" height="777" alt="image" src="https://github.com/user-attachments/assets/9ca5fca6-1655-4a0d-9cb1-3a70471696b1" />
<img width="1547" height="592" alt="image" src="https://github.com/user-attachments/assets/f81db377-8d14-49b2-8916-2400ae16de17" />
<img width="1591" height="536" alt="image" src="https://github.com/user-attachments/assets/a587b20b-d981-4081-9c9d-a453999a3d0d" />

Ayrıca kurye ve eczaneler sisteme direkt olarak kaydolamaz, başvuru isteği yollarlar ve admin onaylarsa sisteme kayıt olabilirler

<img width="808" height="738" alt="Ekran görüntüsü 2026-05-20 132811" src="https://github.com/user-attachments/assets/d56a977a-a45b-4b4a-a7cf-63e830f5a8c4" />

<img width="619" height="792" alt="Ekran görüntüsü 2026-05-20 132514" src="https://github.com/user-attachments/assets/d6f5207c-2720-4cb5-ab1d-3e091ecf7825" />

<img width="1277" height="285" alt="Ekran görüntüsü 2026-05-20 132540" src="https://github.com/user-attachments/assets/3473a207-306d-42cc-adf8-a4f22a9b68f5" />

Admin ekranında görüntüsü: 

<img width="1050" height="506" alt="Ekran görüntüsü 2026-05-20 132613" src="https://github.com/user-attachments/assets/49008c40-fe16-4c36-8c6e-ebbff3ed1e0c" />

<img width="1019" height="559" alt="Ekran görüntüsü 2026-05-20 132624" src="https://github.com/user-attachments/assets/94daec03-5760-4275-9d67-723646fdeec2" />

Kurye için admin paneli:

<img width="636" height="456" alt="Ekran görüntüsü 2026-05-20 132836" src="https://github.com/user-attachments/assets/96f1cb5d-d33c-45cc-8b17-5d4fef3be9c4" /> <img width="649" height="497" alt="Ekran görüntüsü 2026-05-20 132840" src="https://github.com/user-attachments/assets/492d88dc-4e82-4a6e-b6a6-2aa7fd1b3ab1" />

projede her isteğin kullandığı cpu, kaplanan memory, ne kadar sürdüğü gibi analizler için monitoring uygulandı. Monitoring işlemi için prometheus ve görselleştirme için grafana kulllanıldı 

<img width="1600" height="859" alt="image" src="https://github.com/user-attachments/assets/038c727a-eb56-4b5e-9f12-11a91d484d92" />

<img width="1435" height="690" alt="Ekran görüntüsü 2026-05-21 151115" src="https://github.com/user-attachments/assets/a21541db-6232-433c-b1b8-b694c839e8b8" />



