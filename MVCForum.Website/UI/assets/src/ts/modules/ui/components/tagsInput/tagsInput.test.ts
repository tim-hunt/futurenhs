import { TagsInput } from './index';

const mockedTagsInput = jest.fn();

beforeEach(() => {

    (jQuery.fn as any).tagsInput = mockedTagsInput;

    document.body.innerHTML = `
        <div id="main">
            <textarea id="tagstextarea"></textarea>
            <div>
                <input id="generatedInput">
            </div>
        </div>`;

});



describe('Tags input', () => {

    it('Binds successfully', () => {

        const textareaElement: HTMLElement = document.getElementById('tagstextarea');
        const tagsInputInstance: TagsInput = new TagsInput({
            wrapperSelector: textareaElement,
        });

        expect(tagsInputInstance).toBeTruthy();
        expect(tagsInputInstance.wrapperSelector).toBe(textareaElement);

    });


    it('Binds additional input tags config ', () => {

        const textareaElement: HTMLElement = document.getElementById('tagstextarea');
        const tagsInputAdditionalConfig = {
            minChars: 2,
            maxChars: 25,
            removeWithBackspace: true,
            autocomplete_url: '/tag/autocompletetags',
        };
        const tagsInput: TagsInput = new TagsInput({
            wrapperSelector: textareaElement,
            tagsInputAdditionalConfig: tagsInputAdditionalConfig,
        });

        expect(tagsInput.tagsInputAdditionalConfig).toBe(tagsInputAdditionalConfig);

    });

    it('Inits tags input', () => {

        
        const generatedInput: HTMLElement = document.getElementById('generatedInput');

        const mockedTextareaElement =  { 
            nextSibling:  {
                getElementsByTagName: jest.fn((tagName) => [generatedInput])
            } 
        };

        const tagsInputInstance: TagsInput = new TagsInput({
            wrapperSelector:  <any>mockedTextareaElement,
        });

        tagsInputInstance.init();

        expect(mockedTagsInput).toBeCalledTimes(1);

    });



})


