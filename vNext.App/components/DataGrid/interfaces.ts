﻿export type DataCell = {
    children: any;
    className?: string;
};

export type DataRow = Array<DataCell>;

export interface Props {
    columnList: DataRow;
    rowList: Array<DataRow>
    text: {
        caption: string;
    };
    shouldRenderCaption?: boolean;
    id?: string;
    className?: string;
}