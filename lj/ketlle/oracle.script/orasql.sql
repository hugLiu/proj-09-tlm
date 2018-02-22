SELECT
JLID AS ID,
JLID AS IIID, 
sysdate as IndexedDate,
'' AS Thumbnail,--thumbnail,
'' as FullText,--fulltext,
/*source*/
XXBTURL AS SourceURL,--source.url
'XXGXPT_普联' AS SourceType,--source.type
'信息共享平台_普联' AS SourceName,--source.name
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
'张三' as DCContributorName,--dc.contributor.name
JLMC as DCSubject,--dc.subject
'Catalogue' as DCDescriptionType,--dc.description.type 
XXBTPZMC as DCDescriptionText,--dc.description.text 
'Create' AS DCDateType1,--dc.date.type
JLCSSJ AS DCDateValue1,--dc.date.value
'Issued' AS DCDateType2,--dc.date.type
DATEZDZ AS DCDateValue2,--dc.date.value
BTFLMC AS DCType,--dc.type
'中文' AS DCLanguage,--dc.language
'已审核' AS DCstatus--dc.status
FROM TLMTESTDATA.XXGXPT_XXPT_JL_B T
