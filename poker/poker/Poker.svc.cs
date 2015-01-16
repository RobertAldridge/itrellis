
// file name: poker.svc.cs

using ArgumentNullException = System.ArgumentNullException;
using Console = System.Console;
using DataContractJsonSerializer = System.Runtime.Serialization.Json.DataContractJsonSerializer;
using Debugger = System.Diagnostics.Debugger;
using DecimalList = System.Collections.Generic.List<decimal>;
using PokerHandList = System.Collections.Generic.List<Poker.Poker.PokerHand>;
using Math = System.Math;
using MemoryStream = System.IO.MemoryStream;
using ReaderOptions = System.Xml.Linq.ReaderOptions;
using StreamReader = System.IO.StreamReader;
//using XmlDocument = System.Xml.XmlDocument;
using XElement = System.Xml.Linq.XElement;
using XmlReader = System.Xml.XmlReader;
using XmlReaderSettings = System.Xml.XmlReaderSettings;
using XmlTextReader = System.Xml.XmlTextReader;
using WhitespaceHandling = System.Xml.WhitespaceHandling;

namespace Poker
{
    public class Poker : IPoker
    {
        public struct PokerCard
        {
            public int value;
            public int suit;

            PokerCard(int newValue, int newSuit)
            {
                value = newValue;
                suit = newSuit;
            }

            public static bool isConsecutiveValue(PokerCard lhs, PokerCard rhs)
            {
                return lhs.value+1 == rhs.value;
            }

            public static bool isSameValue(PokerCard lhs, PokerCard rhs)
            {
                return lhs.value == rhs.value;
            }

            public static bool isSameSuit(PokerCard lhs, PokerCard rhs)
            {
                return lhs.suit == rhs.suit;
            }
        }
        public struct PokerHand
        {
            public string name;

            // hand must be sorted
            public PokerCard[] cards;

            PokerHand(string newName, PokerCard[] newCards)
            {
                name = newName;

                cards = new PokerCard[newCards.Length];

                for(int index = 0; index < cards.Length; index++)
                {
                    cards[index] = newCards[index];
                }
            }
            public int lowestPairIndex()
            {
                for(int index = 1; index < cards.Length; index++)
                {
                    if(PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return index-1;
                    }
                }

                return 0;
            }
            public int highestPairIndex()
            {
                for(int index = cards.Length-1; index > 0; index--)
                {
                    if(PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return index-1;
                    }
                }

                return 0;
            }
            public int lowestPairValue()
            {
                for(int index = 1; index < cards.Length; index++)
                {
                    if(PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return cards[index-1].value;
                    }
                }

                return 0;
            }
            public int highestPairValue()
            {
                for(int index = cards.Length-1; index > 0; index--)
                {
                    if(PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return cards[index-1].value;
                    }
                }

                return 0;
            }
            public bool isAnyTwo()
            {
                for(int index = 1; index < cards.Length; index++)
                {
                    if(PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return true;
                    }
                }

                return false;
            }
            public bool isOnlyPair()
            {
                return isAnyTwo() && lowestPairIndex() == highestPairIndex();
            }            
            public bool isAnyTwoOfTwo()
            {
                return lowestPairIndex() != highestPairIndex() && lowestPairValue() != highestPairValue();
            }
            public bool isOnlyTwoPairs()
            {
                return isAnyTwoOfTwo() && !isAnyThree();
            }
            public bool isAnyThree()
            {
                for(int index = 2; index < cards.Length; index++)
                {
                    if(PokerCard.isSameValue(cards[index-2], cards[index-1] ) && PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return true;
                    }
                }

                return false;
            }
            public bool isOnlyThreeOfAKind()
            {
                return isAnyThree() && !isAnyFour() && !isFullHouse();
            }
            public bool isStraight()
            {
                for(int index = 1; index < cards.Length; index++)
                {
                    if(!PokerCard.isConsecutiveValue(cards[index-1], cards[index] ) )
                    {
                        return false;
                    }
                }

                return true;
            }
            public bool isFlush()
            {
                for(int index = 1; index < cards.Length; index++)
                {
                    if(!PokerCard.isSameSuit(cards[index-1], cards[index] ) )
                    {
                        return false;
                    }
                }

                return true;
            }
            public bool isFullHouse()
            {
                return isAnyThree() && isAnyTwoOfTwo();
            }
            public bool isAnyFour()
            {
                for(int index = 3; index < cards.Length; index++)
                {
                    if(PokerCard.isSameValue(cards[index-3], cards[index-2] ) && PokerCard.isSameValue(cards[index-2], cards[index-1] ) && PokerCard.isSameValue(cards[index-1], cards[index] ) )
                    {
                        return true;
                    }
                }

                return false;
            }
            public bool isFourOfAKind()
            {
                return isAnyFour();
            }
            public bool isStraightFlush()
            {
                if(isStraight() && isFlush() )
                {
                    return true;
                }

                return false;
            }

            public static int CompareHighCard(PokerHand lhs, PokerHand rhs)
            {
                for(int index = 4; index >= 0; index--)
                {
                    if(lhs.cards[index].value > rhs.cards[index].value)
                    {
                        return -1;
                    }
                    if(lhs.cards[index].value < rhs.cards[index].value)
                    {
                        return 1;
                    }
                }

                return 0;
            }

            public bool whatValueOnePair(ref int inside2, ref int outsideHigh1, ref int outsideMedium1, ref int outsideLow1)
            {
                if(PokerCard.isSameValue(cards[0], cards[1] ) )
                {
                    inside2 = cards[1].value;
                    outsideHigh1 = cards[4].value;
                    outsideMedium1 = cards[3].value;
                    outsideLow1 = cards[2].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[1], cards[2] ) )
                {
                    inside2 = cards[2].value;
                    outsideHigh1 = cards[4].value;
                    outsideMedium1 = cards[3].value;
                    outsideLow1 = cards[0].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[2], cards[3] ) )
                {
                    inside2 = cards[3].value;
                    outsideHigh1 = cards[4].value;
                    outsideMedium1 = cards[1].value;
                    outsideLow1 = cards[0].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[3], cards[4] ) )
                {
                    inside2 = cards[4].value;
                    outsideHigh1 = cards[2].value;
                    outsideMedium1 = cards[1].value;
                    outsideLow1 = cards[0].value;
                    return true;
                }

                return false;
            }
            public static int ComparePair(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isOnlyPair() || !rhs.isOnlyPair() )
                {
                    return 0;
                }

                int lhsIn2 = 0;
                int lhsOutHi1 = 0;
                int lhsOutMe1 = 0;
                int lhsOutLo1 = 0;
                lhs.whatValueOnePair(ref lhsIn2, ref lhsOutHi1, ref lhsOutMe1, ref lhsOutLo1);

                int rhsIn2 = 0;
                int rhsOutHi1 = 0;
                int rhsOutMe1 = 0;
                int rhsOutLo1 = 0;
                rhs.whatValueOnePair(ref rhsIn2, ref rhsOutHi1, ref rhsOutMe1, ref rhsOutLo1);

                if(lhsIn2 > rhsIn2)
                {
                    return -1;
                }
                if(lhsIn2 < rhsIn2)
                {
                    return 1;
                }

                if(lhsOutHi1 > rhsOutHi1)
                {
                    return -1;
                }
                if(lhsOutHi1 < rhsOutHi1)
                {
                    return 1;
                }

                if(lhsOutMe1 > rhsOutMe1)
                {
                    return -1;
                }
                if(lhsOutMe1 < rhsOutMe1)
                {
                    return 1;
                }

                if(lhsOutLo1 > rhsOutLo1)
                {
                    return -1;
                }
                if(lhsOutLo1 < rhsOutLo1)
                {
                    return 1;
                }

                return 0;
            }

            public bool whatValueTwoPairs(ref int insideHigh2, ref int insideLow2, ref int outside)
            {
                if(PokerCard.isSameValue(cards[0], cards[1]) && PokerCard.isSameValue(cards[2], cards[3]))
                {
                    insideHigh2 = cards[3].value;
                    insideLow2 = cards[1].value;
                    outside = cards[4].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[0], cards[1]) && PokerCard.isSameValue(cards[3], cards[4]))
                {
                    insideHigh2 = cards[4].value;
                    insideLow2 = cards[1].value;
                    outside = cards[2].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[1], cards[2]) && PokerCard.isSameValue(cards[3], cards[4]))
                {
                    insideHigh2 = cards[4].value;
                    insideLow2 = cards[2].value;
                    outside = cards[0].value;
                    return true;
                }

                return false;
            }
            public static int CompareTwoPairs(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isOnlyTwoPairs() || !rhs.isOnlyTwoPairs())
                {
                    return 0;
                }

                int lhsInHi2 = 0;
                int lhsInLo2 = 0;
                int lhsOut = 0;
                lhs.whatValueTwoPairs(ref lhsInHi2, ref lhsInLo2, ref lhsOut);

                int rhsInHi2 = 0;
                int rhsInLo2 = 0;
                int rhsOut = 0;
                rhs.whatValueTwoPairs(ref rhsInHi2, ref rhsInLo2, ref rhsOut);

                if(lhsInHi2 > rhsInHi2)
                {
                    return -1;
                }
                if(lhsInHi2 < rhsInHi2)
                {
                    return 1;
                }

                if(lhsInLo2 > rhsInLo2)
                {
                    return -1;
                }
                if(lhsInLo2 < rhsInLo2)
                {
                    return 1;
                }

                if(lhsOut > rhsOut)
                {
                    return -1;
                }
                if(lhsOut < rhsOut)
                {
                    return 1;
                }

                return 0;
            }
            public bool whatValueThreeOfAKind(ref int inside3, ref int outsideHigh, ref int outsideLow)
            {
                if(PokerCard.isSameValue(cards[0], cards[1] ) && PokerCard.isSameValue(cards[1], cards[2] ) )
                {
                    inside3 = cards[2].value;
                    outsideHigh = cards[4].value;
                    outsideLow = cards[3].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[1], cards[2] ) && PokerCard.isSameValue(cards[2], cards[3] ) )
                {
                    inside3 = cards[3].value;
                    outsideHigh = cards[4].value;
                    outsideLow = cards[0].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[2], cards[3] ) && PokerCard.isSameValue(cards[3], cards[4] ) )
                {
                    inside3 = cards[4].value;
                    outsideHigh = cards[1].value;
                    outsideLow = cards[0].value;
                    return true;
                }

                return false;
            }
            public static int CompareThreeOfAKind(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isOnlyThreeOfAKind() || !rhs.isOnlyThreeOfAKind() )
                {
                    return 0;
                }

                int lhsIn3 = 0;
                int lhsOut1 = 0;
                int lhsOut2 = 0;
                lhs.whatValueThreeOfAKind(ref lhsIn3, ref lhsOut1, ref lhsOut2);

                int rhsIn3 = 0;
                int rhsOut1 = 0;
                int rhsOut2 = 0;
                rhs.whatValueThreeOfAKind(ref rhsIn3, ref rhsOut1, ref rhsOut2);

                if(lhsIn3 > rhsIn3)
                {
                    return -1;
                }
                if(lhsIn3 < rhsIn3)
                {
                    return 1;
                }

                if(lhsOut1 > rhsOut1)
                {
                    return -1;
                }
                if(lhsOut1 < rhsOut1)
                {
                    return 1;
                }

                if(lhsOut2 > rhsOut2)
                {
                    return -1;
                }
                if(lhsOut2 < rhsOut2)
                {
                    return 1;
                }

                return 0;
            }
            public static int CompareStraight(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isStraight() || !rhs.isStraight() )
                {
                    return 0;
                }

                if(lhs.cards[4].value > rhs.cards[4].value)
                {
                    return -1;
                }
                if(lhs.cards[4].value < rhs.cards[4].value)
                {
                    return 1;
                }

                return 0;
            }
            public static int CompareFlush(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isFlush() || !rhs.isFlush())
                {
                    return 0;
                }

                for(int index = 4; index >= 0; index--)
                {
                    if(lhs.cards[index].value > rhs.cards[index].value)
                    {
                        return -1;
                    }
                    if(lhs.cards[index].value < rhs.cards[index].value)
                    {
                        return 1;
                    }
                }

                return 0;
            }
            public bool whatValueFullHouse(ref int inside3, ref int inside2)
            {
                if(PokerCard.isSameValue(cards[0], cards[1] ) && PokerCard.isSameValue(cards[1], cards[2] ) )
                {
                    inside3 = cards[2].value;
                    inside2 = cards[4].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[2], cards[3] ) && PokerCard.isSameValue(cards[3], cards[4] ) )
                {
                    inside3 = cards[4].value;
                    inside2 = cards[1].value;
                    return true;
                }

                return false;
            }
            public static int CompareFullHouse(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isFullHouse() || !rhs.isFullHouse() )
                {
                    return 0;
                }

                int lhsIn3 = 0;
                int lhsIn2 = 0;
                lhs.whatValueFullHouse(ref lhsIn3, ref lhsIn2);

                int rhsIn3 = 0;
                int rhsIn2 = 0;
                rhs.whatValueFullHouse(ref rhsIn3, ref rhsIn2);

                if(lhsIn3 > rhsIn3)
                {
                    return -1;
                }
                if(lhsIn3 < rhsIn3)
                {
                    return 1;
                }

                if(lhsIn2 > rhsIn2)
                {
                    return -1;
                }
                if(lhsIn2 < rhsIn2)
                {
                    return 1;
                }

                return 0;
            }
            public bool whatValueFourOfAKind(ref int inside, ref int outside)
            {
                if(PokerCard.isSameValue(cards[0], cards[1] ) && PokerCard.isSameValue(cards[1], cards[2] ) && PokerCard.isSameValue(cards[2], cards[3] ) )
                {
                    inside = cards[3].value;
                    outside = cards[4].value;
                    return true;
                }

                if(PokerCard.isSameValue(cards[1], cards[2] ) && PokerCard.isSameValue(cards[2], cards[3] ) && PokerCard.isSameValue(cards[3], cards[4] ) )
                {
                    inside = cards[4].value;
                    outside = cards[0].value;
                    return true;
                }

                return false;
            }
            public static int CompareFourOfAKind(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isFourOfAKind() || !rhs.isFourOfAKind() )
                {
                    return 0;
                }

                int lhsIn = 0;
                int lhsOut = 0;
                lhs.whatValueFourOfAKind(ref lhsIn, ref lhsOut);

                int rhsIn = 0;
                int rhsOut = 0;
                rhs.whatValueFourOfAKind(ref rhsIn, ref rhsOut);

                if(lhsIn > rhsIn)
                {
                    return -1;
                }
                if(lhsIn < rhsIn)
                {
                    return 1;
                }

                if(lhsOut > rhsOut)
                {
                    return -1;
                }
                if(lhsOut < rhsOut)
                {
                    return 1;
                }

                return 0;
            }
            public static int CompareStraightFlush(PokerHand lhs, PokerHand rhs)
            {
                if(!lhs.isStraightFlush() || !rhs.isStraightFlush() )
                {
                    return 0;
                }

                if(lhs.cards[4].value > rhs.cards[4].value)
                {
                    return -1;
                }
                if(lhs.cards[4].value < rhs.cards[4].value)
                {
                    return 1;
                }


                return 0;
            }
        }

        private static void PokerHandSort(ref PokerHand thePokerHand)
        {
            for(int indexLhs = 0; indexLhs<=3; indexLhs++)
            {
                for(int indexRhs = indexLhs+1; indexRhs<=4; indexRhs++)
                {
                    if(thePokerHand.cards[indexLhs].value > thePokerHand.cards[indexRhs].value)
                    {
                        PokerCard temp = thePokerHand.cards[indexLhs];
                        thePokerHand.cards[indexLhs] = thePokerHand.cards[indexRhs];
                        thePokerHand.cards[indexRhs] = temp;
                    }
                }
            }
        }

        public XElement OminousPokerFunction(XElement input)
        {
            XmlReader xmlReader = input.CreateReader(ReaderOptions.None);

            xmlReader.MoveToContent();

            PokerHandList pokerHands = new PokerHandList();

            xmlReader.Read();

            int index = 0;
            while(!xmlReader.EOF)
            {
                while(!xmlReader.EOF && xmlReader.Name != "name")
                    xmlReader.Read();
                if(!xmlReader.EOF)
                    xmlReader.MoveToContent();

                if(xmlReader.EOF)
                    break;

                PokerHand newHand;
                newHand.cards = new PokerCard[5];
                newHand.name = xmlReader.ReadInnerXml();

                while(!xmlReader.EOF && xmlReader.Name != "card")
                     xmlReader.Read();
                if(!xmlReader.EOF)
                    xmlReader.MoveToContent();

                index = 0;
                for(index = 0; index<5 && !xmlReader.EOF; index++)
                {
                    string value = xmlReader.ReadInnerXml();
                    if(!xmlReader.EOF)
                        xmlReader.MoveToContent();

                    switch(value[0].ToString() )
                    {
                    case "2":newHand.cards[index].value = 2;break;
                    case "3":newHand.cards[index].value = 3;break;
                    case "4":newHand.cards[index].value = 4;break;
                    case "5":newHand.cards[index].value = 5;break;
                    case "6":newHand.cards[index].value = 6;break;
                    case "7":newHand.cards[index].value = 7;break;
                    case "8":newHand.cards[index].value = 8;break;
                    case "9":newHand.cards[index].value = 9;break;
                    case "X":newHand.cards[index].value = 10;break;
                    case "J":newHand.cards[index].value = 11;break; // J
                    case "Q":newHand.cards[index].value = 12;break; // Q
                    case "K":newHand.cards[index].value = 13;break; // K
                    case "A":newHand.cards[index].value = 14;break; // A
                    default:Debugger.Break();break;
                    }

                    switch(value[1].ToString() )
                    {
                    case "C":newHand.cards[index].suit = 1;break; // clover
                    case "H":newHand.cards[index].suit = 2;break; // hearts
                    case "S":newHand.cards[index].suit = 3;break; // spades
                    case "D":newHand.cards[index].suit = 4;break; // diamonds
                    default:Debugger.Break();break;
                    }
                }

                if(index!=0 && index!=5)
                    Debugger.Break();

                PokerHandSort(ref newHand);

                pokerHands.Add(newHand);
            }

            if(index!=0 && index!=5)
                Debugger.Break();

            xmlReader.Dispose();

            return CompareHands(pokerHands);
        }

        private static XElement CompareHands(PokerHandList hands)
        {
            string xml =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
            "<results xmlns=\"\">\r\n" +
            "  <result>#USER# #STATUS#</result>\r\n" +
            "</results>";

            PokerHandList[] bucketSort = new PokerHandList[9];
            for(int index = 0; index<bucketSort.Length; index++)
            {
                bucketSort[index] = new PokerHandList();
            }

            foreach(PokerHand hand in hands)
            {
                if(hand.isStraightFlush() ) bucketSort[0].Add(hand);
                else if(hand.isFourOfAKind() ) bucketSort[1].Add(hand);
                else if(hand.isFullHouse() ) bucketSort[2].Add(hand);
                else if(hand.isFlush() ) bucketSort[3].Add(hand);
                else if(hand.isStraight() ) bucketSort[4].Add(hand);
                else if(hand.isOnlyThreeOfAKind() ) bucketSort[5].Add(hand);
                else if(hand.isOnlyTwoPairs() ) bucketSort[6].Add(hand);
                else if(hand.isOnlyPair() ) bucketSort[7].Add(hand);
                else bucketSort[8].Add(hand);
            }

            for(int sortIndex = 0; sortIndex<bucketSort.Length; sortIndex++)
            {
                PokerHandList bucket = bucketSort[sortIndex];
                if(bucket.Count > 0)
                {
                    int highestHandIndex = 0;
                    PokerHand highestHand = bucket[0];

                    if(bucket.Count == 1)
                    {
                        xml = xml.Replace("#USER#", highestHand.name);
                        xml = xml.Replace("#STATUS#", "wins");
                        return XElement.Parse(xml);
                    }

                    int compareResult = 0;
                    for(int bucketIndex = 1; bucketIndex<bucket.Count; bucketIndex++)
                    {
                        switch(sortIndex)
                        {
                        case 0: compareResult = PokerHand.CompareStraightFlush(highestHand, bucket[bucketIndex] ); break;
                        case 1: compareResult = PokerHand.CompareFourOfAKind(highestHand, bucket[bucketIndex] ); break;
                        case 2: compareResult = PokerHand.CompareFullHouse(highestHand, bucket[bucketIndex] ); break;
                        case 3: compareResult = PokerHand.CompareFlush(highestHand, bucket[bucketIndex] ); break;
                        case 4: compareResult = PokerHand.CompareStraight(highestHand, bucket[bucketIndex] ); break;
                        case 5: compareResult = PokerHand.CompareThreeOfAKind(highestHand, bucket[bucketIndex] ); break;
                        case 6: compareResult = PokerHand.CompareTwoPairs(highestHand, bucket[bucketIndex] ); break;
                        case 7: compareResult = PokerHand.ComparePair(highestHand, bucket[bucketIndex] ); break;
                        case 8: compareResult = PokerHand.CompareHighCard(highestHand, bucket[bucketIndex] ); break;
                        }

                        if(compareResult == 0)
                        {
                            highestHandIndex = -1;
                        }
                        else if(compareResult < 0)
                        {
                            highestHandIndex = 0;
                        }
                        else if(compareResult > 0)
                        {
                            highestHandIndex = bucketIndex;
                            highestHand = bucket[highestHandIndex];
                        }
                    }

                    if(highestHandIndex == -1)
                    {
                        // a game where the highest ranking hand is tied for more than one person is a loss
                        xml = xml.Replace("#USER#", "A");
                        xml = xml.Replace("#STATUS#", "TIE");
                        return XElement.Parse(xml);
                    }

                    xml = xml.Replace("#USER#", highestHand.name);
                    xml = xml.Replace("#STATUS#", "wins");
                    return XElement.Parse(xml);
                }
            }

            xml = xml.Replace("#USER#", "NO");
            xml = xml.Replace("#STATUS#", "PLAYERS");
            return XElement.Parse(xml);
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if(composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if(composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
