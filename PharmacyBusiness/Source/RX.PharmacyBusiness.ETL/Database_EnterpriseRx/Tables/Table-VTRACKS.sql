
  CREATE TABLE "WEGMANS"."VTRACKS" 
   (	"STORE_NUM" VARCHAR2(3 CHAR), 
	"LOC_SHIP_COUNTY" VARCHAR2(30 CHAR), 
	"LOC_STORE_NO" VARCHAR2(6 CHAR), 
	"LOC_PROVIDER_PIN" VARCHAR2(9 CHAR)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 1048576 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DW_LARGE_DATA01" ;

   COMMENT ON TABLE "WEGMANS"."VTRACKS"  IS 'Regulatory reporting of COVID vaccinations to the CDC requires Store County plus VTracK PINs.';


INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('001','Onondaga','W00001','TP1W00001');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('003','Monroe','W00002','TP1W00002');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('004','Monroe','W00003','TP1W00003');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('006','Wayne','W00004','TP1W00004');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('007','Loudoun','W00005','TP1W00005');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('008','Burlington','W00006','TP1W00006');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('009','Monmouth','W00007','TP1W00007');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('010','Camden','W00008','TP1W00008');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('011','Ontario','W00009','TP1W00009');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('012','Monroe','W00010','TP1W00010');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('013','Monroe','W00011','TP1W00011');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('014','Baltimore','W00012','TP1W00012');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('016','Fairfax','W00013','TP1W00013');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('017','Steuben','W00014','TP1W00014');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('018','Monroe','W00015','TP1W00015');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('019','Monroe','W00016','TP1W00016');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('020','Monroe','W00017','TP1W00017');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('022','Monroe','W00018','TP1W00018');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('024','Monroe','W00019','TP1W00019');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('025','Monroe','W00020','TP1W00020');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('026','Livingston','W00021','TP1W00021');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('030','Onondaga','W00022','TP1W00022');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('031','Onondaga','W00023','TP1W00023');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('032','Middlesex','W00024','TP1W00024');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('033','Onondaga','W00025','TP1W00025');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('034','Onondaga','W00026','TP1W00026');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('035','Onondaga','W00027','TP1W00027');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('036','Buck','W00028','TP1W00028');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('037','Onondaga','W00029','TP1W00029');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('038','Cayuga','W00030','TP1W00030');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('039','Onondaga','W00031','TP1W00031');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('040','Prince Georges','W00032','TP1W00032');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('041','Spotsylvania','W00033','TP1W00033');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('042','Prince William','W00034','TP1W00034');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('043','Montgomery','W00035','TP1W00035');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('044','Loudoun','W00036','TP1W00036');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('045','Cumberland','W00037','TP1W00037');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('046','Chester','W00038','TP1W00038');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('047','Howard','W00039','TP1W00039');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('048','Montgomery','W00040','TP1W00040');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('049','Fairfax','W00041','TP1W00041');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('050','Chester','W00042','TP1W00042');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('051','Steuben','W00043','TP1W00043');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('053','Hartford ','W00044','TP1W00044');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('054','Frederick','W00045','TP1W00045');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('055','Prince William','W00046','TP1W00046');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('056','Montgomery','W00047','TP1W00047');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('057','Norfolk','W00048','TP1W00048');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('058','Worcester','W00049','TP1W00049');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('059','Middlesex','W00050','TP1W00050');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('060','Anne Arundel','W00051','TP1W00051');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('062','Monroe','W00052','TP1W00052');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('063','Monroe','W00053','TP1W00053');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('064','Monroe','W00054','TP1W00054');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('065','Monroe','W00055','TP1W00055');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('066','Monroe','W00056','TP1W00056');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('067','Monroe','W00057','TP1W00057');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('068','Monroe','W00058','TP1W00058');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('069','Erie','W00059','TP1W00059');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('070','Chemung','W00060','TP1W00060');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('071','Tompkins','W00061','TP1W00061');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('073','Broome','W00062','TP1W00062');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('074','Ontario','W00063','TP1W00063');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('075','Erie','W00064','TP1W00064');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('076','Lackawanna','W00065','TP1W00065');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('077','Luzerne','W00066','TP1W00066');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('078','Lycoming','W00067','TP1W00067');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('079','LeHigh','W00068','TP1W00068');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('080','Erie','W00069','TP1W00069');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('082','Erie','W00070','TP1W00070');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('083','Erie','W00071','TP1W00071');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('084','Erie','W00072','TP1W00072');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('086','Erie','W00073','TP1W00073');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('087','Erie','W00074','TP1W00074');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('088','Chatauqua','W00075','TP1W00075');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('089','Erie','W00076','TP1W00076');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('090','Erie','W00077','TP1W00077');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('091','Erie','W00078','TP1W00078');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('092','Niagara','W00079','TP1W00079');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('093','Mercer','W00080','TP1W00080');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('094','North Hampton','W00081','TP1W00081');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('095','Monmouth','W00082','TP1W00082');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('096','Somerset','W00083','TP1W00083');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('097','North Hampton','W00084','TP1W00084');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('098','Centre','W00085','TP1W00085');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('104','Montgomery','W00086','TP1W00086');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('105','Bergen','W00087','TP1W00087');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('124','Middlesex','W00088','TP1W00088');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('125','Baltimore','W00089','TP1W00089');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('126','Delaware','W00090','TP1W00090');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('127','Albemarle','W00091','TP1W00091');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('128','Morris','W00092','TP1W00092');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('129','Chesterfield','W00093','TP1W00093');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('130','Henrico','W00094','TP1W00094');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('133','Fairfax','W00095','TP1W00095');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('135','Lancaster','W00096','TP1W00096');
COMMIT;

--These two in UAT only
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('275','TestCounty275','W00275','TP1W00275');
INSERT INTO wegmans.VTRACKS (store_num, loc_ship_county, loc_store_no, loc_provider_pin) VALUES ('299','TestCounty299','W00299','TP1W00299');
COMMIT;
