# Reprocessing Transaction ADR

## Purpose

The document outlines a new feature that allows the Pharmacy team to reprocess a selection of transactions in the off-business hours.

## Proposal

[![](https://mermaid.ink/img/pako:eNp1Uk1v2zAM_SuETh0QA06B7WAMGdIm2A49uemhjXdQLMYWZksZRS0zgvz30Y7SZYcZEMzPx8cnnVTtDapC7Tt_rFtNDE9l5UC-5XZjeyTYkG0apM87WpTRBbi_L_IcMsg_yX-MpgI0gL-QBvgIvXWRMXy_AD2cvvkj9NoNQFh7MgGsA25R3AP5GkOAgMzWNQFY7zr8cr50Pm5L1GYqNZqlXrsGwe-BxQq6ZuuFEHvYjVjazEA7A8dWyExNdaTgCWwYLULH3ZA4rbZLY25h4KV8mqDGvtv4z4gRITq23ZgTZC1nLhLcVvWyhG7wfbOpK81ab18OF_r_WffKU6YfvHV8peHwN99OSXivd2siTwVsJjqh9bEzowQ75COigzybvyvtXTeMpMrr7Od_Rn-4QL7drZ1J9jLLFg_p5iDL4Ksoy_IOuNUO5hJZwGtKS3YKPP7189F_S9cn5irpLeY6yTEWqJmSt9Vra-TtncZEpWTlHitViGk0_ahU5c5SpyP758HVqmCKOFPkY9OqYq-7IF6cpF1Z3ZDuU_T8B7ed7z8?type=png)](https://mermaid.live/edit#pako:eNp1Uk1v2zAM_SuETh0QA06B7WAMGdIm2A49uemhjXdQLMYWZksZRS0zgvz30Y7SZYcZEMzPx8cnnVTtDapC7Tt_rFtNDE9l5UC-5XZjeyTYkG0apM87WpTRBbi_L_IcMsg_yX-MpgI0gL-QBvgIvXWRMXy_AD2cvvkj9NoNQFh7MgGsA25R3AP5GkOAgMzWNQFY7zr8cr50Pm5L1GYqNZqlXrsGwe-BxQq6ZuuFEHvYjVjazEA7A8dWyExNdaTgCWwYLULH3ZA4rbZLY25h4KV8mqDGvtv4z4gRITq23ZgTZC1nLhLcVvWyhG7wfbOpK81ab18OF_r_WffKU6YfvHV8peHwN99OSXivd2siTwVsJjqh9bEzowQ75COigzybvyvtXTeMpMrr7Od_Rn-4QL7drZ1J9jLLFg_p5iDL4Ksoy_IOuNUO5hJZwGtKS3YKPP7189F_S9cn5irpLeY6yTEWqJmSt9Vra-TtncZEpWTlHitViGk0_ahU5c5SpyP758HVqmCKOFPkY9OqYq-7IF6cpF1Z3ZDuU_T8B7ed7z8)

#### ReprocessSettings Azure Table

The new table will have the following fields:

```json
{
    "startDate": DateTimeOffset,
    "endDate" : DateTimeOffset,
    "cursor": string (last processed file name)
}
```

### Timer Trigger

Scheduled to run every 5 minutes during off hours, this Azure Function trigger will:

- Check if there are any reprocess configurations in the Azure reprocess configuration table
- If there is, check the number of transactions currently in `raw-tlog-transactions` queue
- Retrieve 100 (or some other configurable number) minus the number of items currently in `raw-tlog-transactions` queue from the `transactions` blob container:
    - Starting at the value retrieved from the `ReprocessSettings` tables `cursor` value
    - Going no further than the `ReprocessSettings` tables `endDate` value
- Queue those URLs as a message in the `raw-tlog-transactions` queue
