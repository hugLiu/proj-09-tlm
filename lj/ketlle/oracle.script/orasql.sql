SELECT
JLID AS ID,
JLID AS IIID, 
sysdate as IndexedDate,
'' AS Thumbnail,--thumbnail,
'' as FullText,--fulltext,
/*source*/
XXBTURL AS SourceURL,--source.url
'XXGXPT_����' AS SourceType,--source.type
'��Ϣ����ƽ̨_����' AS SourceName,--source.name
'html' AS SourceFormat,--source.format
'URL' AS SourceMedia,--source.media
DATATABLE AS SourceDataTable,--source.datatable
BTDYZD AS SourceTitleFields,--source.title.fields
YWIDKEY AS SourceDataKey,--source.datakey
XXBTPZID AS SourceTitleConfigKey,--source.title.configkey
/*dc*/
'Formal' AS DCTitleType,--dc.title.type
XXBTPZGS AS DCTitleText,--dc.title.text
'Author' as DCContributorType,--dc.contributor.type
'����' as DCContributorName,--dc.contributor.name
JLMC as DCSubject,--dc.subject
'Catalogue' as DCDescriptionType,--dc.description.type 
XXBTPZMC as DCDescriptionText,--dc.description.text 
'Create' AS DCDateType1,--dc.date.type
JLCSSJ AS DCDateValue1,--dc.date.value
'Issued' AS DCDateType2,--dc.date.type
DATEZDZ AS DCDateValue2,--dc.date.value
BTFLMC AS DCType,--dc.type
'����' AS DCLanguage,--dc.language
'�����' AS DCstatus--dc.status
FROM TLMTESTDATA.XXGXPT_XXPT_JL_B T
